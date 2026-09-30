using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record GroupBookingReservationDetached(
        Guid GroupBookingId,
        Guid OrganizerGuestId,
        string GroupName,
        DateTime ReservationDetachedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GroupBookingId;
        string IAuditableEvent.EntityType => "GroupBooking";
        string IAuditableEvent.Summary => $"Reservation detached from Group Booking {GroupName} : {GroupBookingId} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
