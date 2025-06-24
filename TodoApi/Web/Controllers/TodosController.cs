using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Core.Interfaces;
using TodoApi.Web.DTOs;
using TodoApi.Web.Filters;

namespace TodoApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ValidationFilter))]
    public class TodosController : ControllerBase
    {
        private readonly ITodoService _todoService;
        private readonly ILogger<TodosController> _logger;
        private readonly IMapper _mapper;

        public TodosController(
            ITodoService todoService,
            ILogger<TodosController> logger,
            IMapper mapper)
        {
            _todoService = todoService ?? throw new ArgumentNullException(nameof(todoService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<TodoDto>>> GetTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting all todos");
            var todos = await _todoService.GetAllTodosAsync();
            var totalCount = todos.Count();

            var pagedItems = todos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new PagedResultDto<TodoDto>(
                _mapper.Map<IEnumerable<TodoDto>>(pagedItems),
                totalCount,
                page,
                pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDto>> GetTodo(Guid id)
        {
            _logger.LogInformation("Getting todo with id: {Id}", id);
            var todo = await _todoService.GetTodoByIdAsync(id);
            return Ok(_mapper.Map<TodoDto>(todo));
        }

        [HttpPost]
        public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoDto createTodoDto)
        {
            _logger.LogInformation("Creating new todo");
            var todo = await _todoService.CreateTodoAsync(createTodoDto.Title);
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, _mapper.Map<TodoDto>(todo));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(Guid id, UpdateTodoDto updateTodoDto)
        {
            _logger.LogInformation("Updating todo with id: {Id}", id);
            await _todoService.UpdateTodoAsync(id, updateTodoDto.Title, updateTodoDto.IsCompleted);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id)
        {
            _logger.LogInformation("Deleting todo with id: {Id}", id);
            await _todoService.DeleteTodoAsync(id);
            return NoContent();
        }
    }
}