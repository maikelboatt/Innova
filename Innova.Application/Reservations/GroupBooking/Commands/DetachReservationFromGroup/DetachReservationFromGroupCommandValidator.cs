using FluentValidation;

namespace Innova.Application.Reservations.GroupBooking.Commands.DetachReservationFromGroup
{
    public sealed class DetachReservationFromGroupCommandValidator:AbstractValidator<DetachReservationFromGroupCommand>
    {
        public DetachReservationFromGroupCommandValidator()
        {
            RuleFor(gb => gb.GroupBookingId)
                .NotEmpty()
                .WithMessage("Group Booking Id is required");

            RuleFor(gb => gb.ReservationId)
                .NotEmpty()
                .WithMessage("Reservation Id is required");
        }
    }
}
