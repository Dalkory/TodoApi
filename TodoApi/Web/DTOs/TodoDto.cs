using System.ComponentModel.DataAnnotations;

namespace TodoApi.Web.DTOs
{
    public record TodoDto(
        Guid Id,
        string Title,
        bool IsCompleted,
        DateTime CreatedAt);

    public record CreateTodoDto(
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        string Title);

    public record UpdateTodoDto(
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        string Title,
        [Required(ErrorMessage = "IsCompleted is required")]
        bool IsCompleted);

    public record PagedResultDto<T>(
        IEnumerable<T> Items,
        int TotalCount,
        int PageNumber,
        int PageSize);
}