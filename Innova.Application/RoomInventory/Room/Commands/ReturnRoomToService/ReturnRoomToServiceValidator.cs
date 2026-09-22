using FluentValidation;

namespace Innova.Application.RoomInventory.Room.Commands.ReturnRoomToService
{
    public sealed class ReturnRoomToServiceCommandValidator:AbstractValidator<ReturnRoomToServiceCommand>
    {
        public ReturnRoomToServiceCommandValidator()
        {
            RuleFor(c => c.RoomId)
                .NotEmpty();
        }
    }
}
