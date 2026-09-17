using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomTypeAllotmentReserved(
        Guid RoomTypeAllotmentId,
        Guid RoomTypeId,
        DateOnly PeriodStart,
        DateOnly PeriodEnd,
        int RoomsReserved ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
