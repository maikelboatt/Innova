using Innova.Domain.Common;

namespace Innova.Domain.FrontDesk.Events
{
    public sealed record StayCheckedIn(
        Guid StayId,
        Guid ReservationId,
        Guid RoomId,
        string RoomNumber,
        int MaxOccupancy,
        Guid PrimaryOccupant,
        Guid? GroupBookingId,
        DateTime CheckedIn ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => StayId;
        string IAuditableEvent.EntityType => "Stay";
        string IAuditableEvent.Summary => $"Stay {StayId} for room {RoomId} has Checked-in at {CheckedIn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
