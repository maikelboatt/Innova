using FluentValidation;

namespace Innova.Application.RoomInventory.Room.Commands.TakeRoomOutOfService
{
    public sealed class TakeRoomOutOfServiceCommandValidator:AbstractValidator<TakeRoomOutOfServiceCommand>
    {
        public TakeRoomOutOfServiceCommandValidator()
        {
            RuleFor(c => c.RoomId)
                .NotEmpty();
        }
    }
}
