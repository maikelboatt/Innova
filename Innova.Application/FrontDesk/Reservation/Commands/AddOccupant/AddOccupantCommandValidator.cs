using FluentValidation;

namespace Innova.Application.FrontDesk.Reservation.Commands.AddOccupant
{
    public sealed class AddOccupantCommandValidator:AbstractValidator<AddOccupantCommand>
    {
        public AddOccupantCommandValidator()
        {
            RuleFor(s => s.StayId)
                .NotEmpty()
                .WithMessage("Stay Id is required");

            RuleFor(s => s.GuestId)
                .NotEmpty()
                .WithMessage("Guest Id is required");
        }
    }
}
