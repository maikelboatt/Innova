using FluentValidation;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.CompleteTask
{
    public sealed class CompleteTaskCommandValidator:AbstractValidator<CompleteTaskCommand>
    {
        public CompleteTaskCommandValidator()
        {
            RuleFor(t => t.HouseKeepingId)
                .NotEmpty()
                .WithMessage("House Keeping Id is required");
        }
    }
}
