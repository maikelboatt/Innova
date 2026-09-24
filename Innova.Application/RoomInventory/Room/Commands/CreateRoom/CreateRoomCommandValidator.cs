using FluentValidation;

namespace Innova.Application.RoomInventory.Room.Commands.CreateRoom
{
    public sealed class CreateRoomCommandValidator:AbstractValidator<CreateRoomCommand>
    {
        public CreateRoomCommandValidator()
        {
            RuleFor(r => r.RoomNumber)
                .NotEmpty()
                .WithMessage("Room number is required.");

            RuleFor(r => r.RoomTypeId)
                .NotEmpty()
                .WithMessage("Room type is required.");
        }
    }
}
