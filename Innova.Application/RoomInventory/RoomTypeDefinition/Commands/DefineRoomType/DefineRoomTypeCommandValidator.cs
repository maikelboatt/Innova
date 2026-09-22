using FluentValidation;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Commands.DefineRoomType
{
    public sealed class DefineRoomTypeCommandValidator:AbstractValidator<DefineRoomTypeCommand>
    {
        public DefineRoomTypeCommandValidator()
        {
            RuleFor(c => c.RoomName)
                .NotEmpty();
            RuleFor(c => c.MaxOccupancy)
                .GreaterThanOrEqualTo(1);
            RuleFor(c => c.BaseRateAmount)
                .GreaterThanOrEqualTo(0);
            RuleFor(c => c.BaseRateCurrency)
                .NotEmpty();
        }
    }
}
