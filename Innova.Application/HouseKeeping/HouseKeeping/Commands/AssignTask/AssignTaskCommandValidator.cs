using FluentValidation;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.AssignTask
{
    public sealed class AssignTaskCommandValidator:AbstractValidator<AssignTaskCommand>
    {
        public AssignTaskCommandValidator()
        {
            RuleFor(t => t.HouseKeepingTaskId)
                .NotEmpty()
                .WithMessage("House Keeping Id is required");

            RuleFor(t => t.StaffId)
                .NotEmpty()
                .WithMessage("Staff Id is required");
        }
    }
}
