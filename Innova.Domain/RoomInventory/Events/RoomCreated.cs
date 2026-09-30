using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomCreated(
        Guid RoomId,
        string RoomNumber,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => RoomId;
        string IAuditableEvent.EntityType => "Room";
        string IAuditableEvent.Summary => $"Room '{RoomNumber}' | '{RoomId}' has successfully been created for room type {RoomTypeId} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
