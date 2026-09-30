using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record GroupBookingOpened( Guid GroupBookingId, Guid OrganizerGuestId, string GroupName ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GroupBookingId;
        string IAuditableEvent.EntityType => "GroupBooking";
        string IAuditableEvent.Summary => $"Group Booking {GroupName} : {GroupBookingId} Opened at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
