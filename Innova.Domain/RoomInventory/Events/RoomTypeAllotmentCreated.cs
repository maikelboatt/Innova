using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomTypeAllotmentCreated(
        Guid RoomTypeAllotmentId,
        Guid RoomTypeId,
        DateOnly PeriodStart,
        DateOnly PeriodEnd,
        int TotalRooms ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
