using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomTypeAllotmentReserved(
        Guid RoomTypeAllotmentId,
        Guid RoomTypeId,
        DateOnly Date,
        int RoomsReserved ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => RoomTypeAllotmentId;
        string IAuditableEvent.EntityType => "RoomTypeAllotment";
        string IAuditableEvent.Summary => $"Room type allotment  '{RoomTypeAllotmentId}' has successfully been reserved at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
