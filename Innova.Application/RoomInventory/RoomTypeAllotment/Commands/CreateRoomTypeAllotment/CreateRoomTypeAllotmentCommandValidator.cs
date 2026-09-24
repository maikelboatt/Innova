using FluentValidation;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment
{
    public sealed class CreateRoomTypeAllotmentCommandValidator:AbstractValidator<CreateRoomTypeAllotmentCommand>
    {
        public CreateRoomTypeAllotmentCommandValidator()
        {
            RuleFor(c => c.RoomTypeId)
                .NotEmpty()
                .WithMessage("Room type Id is required.");
            RuleFor(c => c.TotalRooms)
                .GreaterThanOrEqualTo(0);

            RuleFor(c => c.CheckOut)
                .GreaterThan(c => c.CheckIn)
                .WithMessage("Check-out date must be after check-in date.");
        }
    }
}
