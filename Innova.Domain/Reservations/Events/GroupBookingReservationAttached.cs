using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record GroupBookingReservationAttached(
        Guid GroupBookingId,
        Guid OrganizerGuestId,
        string GroupName,
        DateTime ReservationAttachedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
