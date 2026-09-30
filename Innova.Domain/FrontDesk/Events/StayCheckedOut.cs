using Innova.Domain.Common;

namespace Innova.Domain.FrontDesk.Events
{
    public sealed record StayCheckedOut(
        Guid StayId,
        Guid ReservationId,
        Guid RoomId,
        string RoomNumber,
        int MaxOccupancy,
        Guid? GroupBookingId,
        DateTime CheckedOut ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => StayId;
        string IAuditableEvent.EntityType => "Stay";
        string IAuditableEvent.Summary => $"Stay {StayId} for room {RoomId} has Checked-out at {CheckedOut:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
