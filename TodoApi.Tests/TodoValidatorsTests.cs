using FluentValidation.TestHelper;
using TodoApi.Web.DTOs;
using TodoApi.Web.Validators;
using Xunit;

namespace TodoApi.Tests.Unit.Validators
{
    public class TodoValidatorsTests
    {
        private readonly CreateTodoDtoValidator _createValidator;
        private readonly UpdateTodoDtoValidator _updateValidator;

        public TodoValidatorsTests()
        {
            _createValidator = new CreateTodoDtoValidator();
            _updateValidator = new UpdateTodoDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateTodoDtoValidator_ShouldHaveError_WhenTitleIsEmpty(string title)
        {
            // Arrange
            var model = new CreateTodoDto(title);

            // Act
            var result = _createValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Title is required");
        }

        [Fact]
        public void CreateTodoDtoValidator_ShouldHaveError_WhenTitleTooLong()
        {
            // Arrange
            var model = new CreateTodoDto(new string('a', 101));

            // Act
            var result = _createValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Title cannot be longer than 100 characters");
        }

        [Fact]
        public void CreateTodoDtoValidator_ShouldNotHaveError_WhenValid()
        {
            // Arrange
            var model = new CreateTodoDto("Valid title");

            // Act
            var result = _createValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void UpdateTodoDtoValidator_ShouldHaveError_WhenIsCompletedMissing()
        {
            // Arrange
            var model = new UpdateTodoDto("Title", false);

            // Act
            var result = _updateValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.IsCompleted);
        }

        [Fact]
        public void UpdateTodoDtoValidator_ShouldNotHaveError_WhenValid()
        {
            // Arrange
            var model = new UpdateTodoDto("Valid title", true);

            // Act
            var result = _updateValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
            result.ShouldNotHaveValidationErrorFor(x => x.IsCompleted);
        }
    }
}