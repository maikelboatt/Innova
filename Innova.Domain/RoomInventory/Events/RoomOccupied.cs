using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomOccupied(
        Guid RoomId,
        string RoomNumber,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId,
        DateTime OccupiedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => RoomId;
        string IAuditableEvent.EntityType => "Room";
        string IAuditableEvent.Summary => $"Room '{RoomNumber}' | '{RoomId}' is currently occupied at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
