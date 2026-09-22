using FluentValidation;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment
{
    public sealed class CreateRoomTypeAllotmentCommandValidator:AbstractValidator<CreateRoomTypeAllotmentCommand>
    {
        public CreateRoomTypeAllotmentCommandValidator()
        {
            RuleFor(c => c.RoomTypeId)
                .NotEmpty();
            RuleFor(c => c.TotalRooms)
                .GreaterThanOrEqualTo(0);

            RuleFor(c => c.StayPeriod.End)
                .GreaterThan(c => c.StayPeriod.Start)
                .WithMessage("Check-out date must be after check-in date.");
        }
    }
}
