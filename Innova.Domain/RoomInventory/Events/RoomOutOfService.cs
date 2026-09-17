using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public record RoomOutOfService(
        Guid RoomId,
        string RoomNumber,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId,
        DateTime OutOfServiceAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
