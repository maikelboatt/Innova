using FluentValidation;

namespace Innova.Application.FrontDesk.Reservation.Commands.CheckIn
{
    public sealed class CheckInCommandValidator:AbstractValidator<CheckInCommand>
    {
        public CheckInCommandValidator()
        {
            RuleFor(s => s.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation Id is required");

            RuleFor(s => s.PrimaryOccupantId)
                .NotEmpty()
                .WithMessage("Guest Id is required");
        }
    }
}
