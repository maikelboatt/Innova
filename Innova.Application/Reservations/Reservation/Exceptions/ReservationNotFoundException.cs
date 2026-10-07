using Innova.Application.Exceptions;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.Reservation.Exceptions
{
    public sealed class ReservationNotFoundException( ReservationId reservationId ):ApplicationExceptions($"Reservation '{reservationId}' was not found.");
}
