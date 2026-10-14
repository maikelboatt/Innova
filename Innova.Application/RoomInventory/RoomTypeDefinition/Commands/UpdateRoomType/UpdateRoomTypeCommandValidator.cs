using FluentValidation;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Commands.UpdateRoomType
{
    public sealed class UpdateRoomTypeCommandValidator:AbstractValidator<UpdateRoomTypeCommand>
    {
        public UpdateRoomTypeCommandValidator()
        {
            RuleFor(c => c.RoomTypeId)
                .NotEmpty()
                .WithMessage("Room Type Id is required.");
            RuleFor(c => c.RoomName)
                .NotEmpty()
                .WithMessage("Room name is required.");
            RuleFor(c => c.MaxOccupancy)
                .GreaterThanOrEqualTo(1);
            RuleFor(c => c.BaseRateAmount)
                .GreaterThanOrEqualTo(0);
        }
    }
}
