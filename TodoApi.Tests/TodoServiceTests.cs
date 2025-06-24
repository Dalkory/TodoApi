using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Core.Entities;
using TodoApi.Core.Exceptions;
using TodoApi.Core.Interfaces;
using TodoApi.Infrastructure.Data;
using TodoApi.Infrastructure.Services;
using Xunit;

namespace TodoApi.Tests.Unit.Services
{
    public class TodoServiceTests : IDisposable
    {
        private readonly Mock<IRepository<TodoItem>> _mockRepository;
        private readonly Mock<ILogger<TodoService>> _mockLogger;
        private readonly TodoContext _context;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _mockRepository = new Mock<IRepository<TodoItem>>();
            _mockLogger = new Mock<ILogger<TodoService>>();

            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            _context = new TodoContext(options);

            _service = new TodoService(
                _mockRepository.Object,
                _mockLogger.Object,
                _context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task GetAllTodosAsync_ReturnsAllItems()
        {
            // Arrange
            var testItems = new List<TodoItem>
            {
                TodoItem.Create("Test 1"),
                TodoItem.Create("Test 2")
            };

            _mockRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(testItems);

            // Act
            var result = await _service.GetAllTodosAsync();

            // Assert
            Assert.Equal(2, result.Count());
            _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsItem_WhenExists()
        {
            // Arrange
            var testItem = TodoItem.Create("Test");
            _mockRepository.Setup(x => x.GetByIdAsync(testItem.Id))
                .ReturnsAsync(testItem);

            // Act
            var result = await _service.GetTodoByIdAsync(testItem.Id);

            // Assert
            Assert.Equal(testItem.Id, result.Id);
            _mockRepository.Verify(x => x.GetByIdAsync(testItem.Id), Times.Once);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ThrowsNotFoundException_WhenNotExists()
        {
            // Arrange
            var testId = Guid.NewGuid();
            _mockRepository.Setup(x => x.GetByIdAsync(testId))
                .ReturnsAsync((TodoItem?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _service.GetTodoByIdAsync(testId));
        }

        [Fact]
        public async Task CreateTodoAsync_CreatesNewItem()
        {
            // Arrange
            var title = "Test";
            _mockRepository.Setup(x => x.AddAsync(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateTodoAsync(title);

            // Assert
            Assert.Equal(title, result.Title);
            Assert.False(result.IsCompleted);
            _mockRepository.Verify(x => x.AddAsync(It.IsAny<TodoItem>()), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_UpdatesExistingItem()
        {
            // Arrange
            var testItem = TodoItem.Create("Original");
            _mockRepository.Setup(x => x.GetByIdAsync(testItem.Id))
                .ReturnsAsync(testItem);
            _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<TodoItem>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateTodoAsync(testItem.Id, "Updated", true);

            // Assert
            Assert.Equal("Updated", testItem.Title);
            Assert.True(testItem.IsCompleted);
            _mockRepository.Verify(x => x.UpdateAsync(testItem), Times.Once);
        }

        [Fact]
        public async Task DeleteTodoAsync_DeletesExistingItem()
        {
            // Arrange
            var testItem = TodoItem.Create("Test");
            _mockRepository.Setup(x => x.GetByIdAsync(testItem.Id))
                .ReturnsAsync(testItem);
            _mockRepository.Setup(x => x.DeleteAsync(testItem))
                .Returns(Task.CompletedTask);

            // Act
            await _service.DeleteTodoAsync(testItem.Id);

            // Assert
            _mockRepository.Verify(x => x.DeleteAsync(testItem), Times.Once);
        }
    }
}