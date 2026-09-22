using FluentValidation;

namespace Innova.Application.RoomInventory.Room.Commands.CreateRoom
{
    public sealed class CreateRoomCommandValidator:AbstractValidator<CreateRoomCommand>
    {
        public CreateRoomCommandValidator()
        {
            RuleFor(r => r.RoomNumber)
                .NotEmpty();

            RuleFor(r => r.RoomTypeId)
                .NotEmpty();
        }
    }
}
