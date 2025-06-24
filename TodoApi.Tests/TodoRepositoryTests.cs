using Microsoft.EntityFrameworkCore;
using TodoApi.Core.Entities;
using TodoApi.Infrastructure.Data;
using TodoApi.Infrastructure.Repositories;

namespace TodoApi.Tests.Unit.Repositories
{
    public class TodoRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<TodoContext> _options;
        private readonly TodoContext _context;
        private readonly Repository<TodoItem> _repository;

        public TodoRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TodoContext(_options);
            _repository = new Repository<TodoItem>(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsItem_WhenItemExists()
        {
            // Arrange
            var item = TodoItem.Create("Test");
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(item.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(item.Id, result.Id);
        }

        [Fact]
        public async Task AddAsync_AddsItemToDatabase()
        {
            // Arrange
            var initialCount = await _context.TodoItems.CountAsync();
            var item = TodoItem.Create("Test");

            // Act
            await _repository.AddAsync(item);
            await _context.SaveChangesAsync();
            var newCount = await _context.TodoItems.CountAsync();

            // Assert
            Assert.Equal(initialCount + 1, newCount);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesItemInDatabase()
        {
            // Arrange
            var item = TodoItem.Create("Original");
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();

            // Act
            item.Update("Updated", true);
            await _repository.UpdateAsync(item);
            await _context.SaveChangesAsync();
            var updatedItem = await _repository.GetByIdAsync(item.Id);

            // Assert
            Assert.Equal("Updated", updatedItem.Title);
            Assert.True(updatedItem.IsCompleted);
        }

        [Fact]
        public async Task DeleteAsync_RemovesItemFromDatabase()
        {
            // Arrange
            var item = TodoItem.Create("Test");
            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();
            var initialCount = await _context.TodoItems.CountAsync();

            // Act
            await _repository.DeleteAsync(item);
            await _context.SaveChangesAsync();
            var newCount = await _context.TodoItems.CountAsync();

            // Assert
            Assert.Equal(initialCount - 1, newCount);
        }
    }
}