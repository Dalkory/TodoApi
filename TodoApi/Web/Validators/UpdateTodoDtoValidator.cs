using FluentValidation;
using TodoApi.Web.DTOs;

namespace TodoApi.Web.Validators
{
    public class UpdateTodoDtoValidator : AbstractValidator<UpdateTodoDto>
    {
        public UpdateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot be longer than 100 characters");

            RuleFor(x => x.IsCompleted)
                .NotNull().WithMessage("IsCompleted is required");
        }
    }
}