using TodoApi.Core.Entities;
using TodoApi.Core.Exceptions;
using TodoApi.Core.Interfaces;
using TodoApi.Infrastructure.Data;

namespace TodoApi.Infrastructure.Services
{
    public class TodoService : ITodoService
    {
        private readonly IRepository<TodoItem> _repository;
        private readonly ILogger<TodoService> _logger;
        private readonly TodoContext _context;

        public TodoService(
            IRepository<TodoItem> repository,
            ILogger<TodoService> logger,
            TodoContext context)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<TodoItem>> GetAllTodosAsync()
        {
            _logger.LogInformation("Getting all todos");
            return await _repository.GetAllAsync();
        }

        public async Task<TodoItem> GetTodoByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting todo with id: {Id}", id);
            var todo = await _repository.GetByIdAsync(id);

            if (todo == null)
            {
                _logger.LogWarning("Todo with id: {Id} not found", id);
                throw new NotFoundException(nameof(TodoItem), id);
            }

            return todo;
        }

        public async Task<TodoItem> CreateTodoAsync(string title)
        {
            _logger.LogInformation("Creating new todo");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var todo = TodoItem.Create(title);
                await _repository.AddAsync(todo);
                await transaction.CommitAsync();
                return todo;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating todo");
                throw;
            }
        }

        public async Task UpdateTodoAsync(Guid id, string title, bool isCompleted)
        {
            _logger.LogInformation("Updating todo with id: {Id}", id);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var todo = await GetTodoByIdAsync(id);
                todo.Update(title, isCompleted);
                await _repository.UpdateAsync(todo);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating todo with id: {Id}", id);
                throw;
            }
        }

        public async Task DeleteTodoAsync(Guid id)
        {
            _logger.LogInformation("Deleting todo with id: {Id}", id);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var todo = await GetTodoByIdAsync(id);
                await _repository.DeleteAsync(todo);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting todo with id: {Id}", id);
                throw;
            }
        }
    }
}