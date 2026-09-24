using FluentValidation;

namespace Innova.Application.Reservations.Reservation.Commands.CancelReservation
{
    public sealed class CancelReservationCommandValidator:AbstractValidator<CancelReservationCommand>
    {
        public CancelReservationCommandValidator()
        {
            RuleFor(r => r.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation Id is required.");
        }
    }
}
