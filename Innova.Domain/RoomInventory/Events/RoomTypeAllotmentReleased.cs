using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomTypeAllotmentReleased(
        Guid RoomTypeAllotmentId,
        Guid RoomTypeId,
        DateOnly PeriodStart,
        DateOnly PeriodEnd,
        int RoomsReleased ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
