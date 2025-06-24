using TodoApi.Core.Entities;

namespace TodoApi.Core.Interfaces
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllTodosAsync();
        Task<TodoItem> GetTodoByIdAsync(Guid id);
        Task<TodoItem> CreateTodoAsync(string title);
        Task UpdateTodoAsync(Guid id, string title, bool isCompleted);
        Task DeleteTodoAsync(Guid id);
    }
}