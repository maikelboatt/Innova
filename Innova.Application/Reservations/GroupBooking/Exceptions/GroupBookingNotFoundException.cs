using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Reservations.GroupBooking.Exceptions
{
    public class GroupBookingNotFoundException( GroupBookingId groupBookingId ):ApplicationException($"Group Booking '{groupBookingId}' was not found.");
}
