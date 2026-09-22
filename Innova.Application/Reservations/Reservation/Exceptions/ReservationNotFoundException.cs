using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.Reservation.Exceptions
{
    public sealed class ReservationNotFoundException( ReservationId reservationId ):ApplicationException($"Reservation '{reservationId}' was not found.");
}
