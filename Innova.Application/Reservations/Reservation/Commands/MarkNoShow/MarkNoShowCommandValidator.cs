using FluentValidation;

namespace Innova.Application.Reservations.Reservation.Commands.MarkNoShow
{
    public sealed class MarkNoShowCommandValidator:AbstractValidator<MarkNoShowCommand>
    {
        public MarkNoShowCommandValidator()
        {
            RuleFor(r => r.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation Id is required.");
        }
    }
}
