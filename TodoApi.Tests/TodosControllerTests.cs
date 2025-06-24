using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Core.Entities;
using TodoApi.Core.Interfaces;
using TodoApi.Infrastructure.Mapping;
using TodoApi.Web.Controllers;
using TodoApi.Web.DTOs;

namespace TodoApi.Tests.Unit.Controllers
{
    public class TodosControllerTests
    {
        private readonly Mock<ITodoService> _mockService;
        private readonly Mock<ILogger<TodosController>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly TodosController _controller;

        public TodosControllerTests()
        {
            _mockService = new Mock<ITodoService>();
            _mockLogger = new Mock<ILogger<TodosController>>();

            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile(new TodoProfile()));
            _mapper = config.CreateMapper();

            _controller = new TodosController(
                _mockService.Object,
                _mockLogger.Object,
                _mapper);
        }

        [Fact]
        public async Task GetTodos_ReturnsAllItems()
        {
            // Arrange
            var testItems = new List<TodoItem>
            {
                TodoItem.Create("Test 1"),
                TodoItem.Create("Test 2")
            };

            _mockService.Setup(x => x.GetAllTodosAsync())
                .ReturnsAsync(testItems);

            // Act
            var result = await _controller.GetTodos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsType<PagedResultDto<TodoDto>>(okResult.Value);
            Assert.Equal(2, pagedResult.Items.Count());
        }

        [Fact]
        public async Task GetTodo_ReturnsItem_WhenExists()
        {
            // Arrange
            var testItem = TodoItem.Create("Test");
            _mockService.Setup(x => x.GetTodoByIdAsync(testItem.Id))
                .ReturnsAsync(testItem);

            // Act
            var result = await _controller.GetTodo(testItem.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<TodoDto>(okResult.Value);
            Assert.Equal(testItem.Id, dto.Id);
        }

        [Fact]
        public async Task CreateTodo_ReturnsCreatedResponse()
        {
            // Arrange
            var createDto = new CreateTodoDto("Test");
            var testItem = TodoItem.Create(createDto.Title);

            _mockService.Setup(x => x.CreateTodoAsync(createDto.Title))
                .ReturnsAsync(testItem);

            // Act
            var result = await _controller.CreateTodo(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TodosController.GetTodo), createdAtActionResult.ActionName);
            Assert.Equal(testItem.Id, ((TodoDto)createdAtActionResult.Value!).Id);
        }

        [Fact]
        public async Task UpdateTodo_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var testId = Guid.NewGuid();
            var updateDto = new UpdateTodoDto("Updated", true);

            _mockService.Setup(x => x.UpdateTodoAsync(testId, updateDto.Title, updateDto.IsCompleted))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTodo(testId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTodo_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var testId = Guid.NewGuid();
            _mockService.Setup(x => x.DeleteTodoAsync(testId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTodo(testId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}