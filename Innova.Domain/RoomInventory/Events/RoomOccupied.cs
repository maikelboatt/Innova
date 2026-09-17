using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomOccupied(
        Guid RoomId,
        string RoomNumber,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId,
        DateTime OccupiedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
