using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomTypeAllotmentReleased(
        Guid RoomTypeAllotmentId,
        Guid RoomTypeId,
        DateOnly Date,
        int RoomsReleased ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => RoomTypeAllotmentId;
        string IAuditableEvent.EntityType => "RoomTypeAllotment";
        string IAuditableEvent.Summary => $"Room type allotment  '{RoomTypeAllotmentId}' has successfully been released at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
