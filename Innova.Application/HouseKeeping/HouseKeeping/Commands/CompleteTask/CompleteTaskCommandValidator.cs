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

            RuleFor(t => t.InspectionNotes)
                .Empty()
                .When(t => t.InspectionPassed)
                .WithMessage("A passed inspection should not include failure notes.");

            RuleFor(t => t.InspectionNotes)
                .NotEmpty()
                .When(t => !t.InspectionPassed)
                .WithMessage("Notes are required when an inspection fails.");
        }
    }
}
