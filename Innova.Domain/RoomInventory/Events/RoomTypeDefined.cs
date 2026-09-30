using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomTypeDefined(
        Guid RoomTypeId,
        string Name,
        int MaxOccupancyValue,
        decimal BaseRateAmount,
        string BaseRateCurrency ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => RoomTypeId;
        string IAuditableEvent.EntityType => "RoomTypeAllotment";
        string IAuditableEvent.Summary => $"Room type {RoomTypeId} with name {Name} has successfully been defined at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
