using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record GroupBookingReservationDetached(
        Guid GroupBookingId,
        Guid OrganizerGuestId,
        string GroupName,
        DateTime ReservationDetachedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
