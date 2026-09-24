using FluentValidation;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.ScheduleTask
{
    public sealed class ScheduleTaskCommandValidator:AbstractValidator<ScheduleTaskCommand>
    {
        public ScheduleTaskCommandValidator()
        {
            RuleFor(t => t.RoomId)
                .NotEmpty()
                .WithMessage("Room Id is required");
        }
    }
}
