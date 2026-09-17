using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomDirtied(
        Guid RoomId,
        string RoomNumber,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId,
        DateTime DirtiedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
