using FluentValidation;

namespace Innova.Application.Reservations.Reservation.Commands.ConfirmReservation
{
    public sealed class ConfirmReservationCommandValidator:AbstractValidator<ConfirmReservationCommand>
    {
        public ConfirmReservationCommandValidator()
        {
            RuleFor(r => r.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation Id is required.");
        }
    }
}
