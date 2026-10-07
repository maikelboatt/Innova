using Innova.Application.Exceptions;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.GroupBooking.Exceptions
{
    public class GroupBookingNotFoundException( GroupBookingId groupBookingId ):ApplicationExceptions($"Group Booking '{groupBookingId}' was not found.");
}
