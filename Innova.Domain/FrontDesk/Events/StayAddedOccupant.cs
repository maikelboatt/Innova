using Innova.Domain.Common;

namespace Innova.Domain.FrontDesk.Events
{
    public sealed record StayAddedOccupant(
        Guid StayId,
        Guid ReservationId,
        Guid RoomId,
        string RoomNumber,
        Guid GuestId,
        int MaxOccupancy,
        DateTime OccupantAddedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => StayId;
        string IAuditableEvent.EntityType => "Stay";
        string IAuditableEvent.Summary => $"Stay {StayId} has added occupant {GuestId} to room {RoomId} at {OccupantAddedAt:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
