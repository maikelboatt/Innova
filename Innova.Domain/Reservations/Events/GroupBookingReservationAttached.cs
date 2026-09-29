using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record GroupBookingReservationAttached(
        Guid GroupBookingId,
        Guid OrganizerGuestId,
        string GroupName,
        DateTime ReservationAttachedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GroupBookingId;
        string IAuditableEvent.EntityType => "GroupBooking";
        string IAuditableEvent.Summary => $"Reservation attached to Group Booking {GroupName} : {GroupBookingId} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
