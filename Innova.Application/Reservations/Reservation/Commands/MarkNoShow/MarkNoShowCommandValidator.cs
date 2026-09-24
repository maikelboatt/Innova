using FluentValidation;
using Innova.Application.Reservations.Reservation.Commands.CancelReservation;

namespace Innova.Application.Reservations.Reservation.Commands.MarkNoShow
{
    public sealed class MarkNoShowCommandValidator:AbstractValidator<CancelReservationCommand>
    {
        public MarkNoShowCommandValidator()
        {
            RuleFor(r => r.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation Id is required.");
        }
    }
}
