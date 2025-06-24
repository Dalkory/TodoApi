using FluentValidation;
using TodoApi.Web.DTOs;

namespace TodoApi.Web.Validators
{
    public class CreateTodoDtoValidator : AbstractValidator<CreateTodoDto>
    {
        public CreateTodoDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(100).WithMessage("Title cannot be longer than 100 characters");
        }
    }
}