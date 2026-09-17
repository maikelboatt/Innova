using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public record RoomBackInService(
        Guid RoomId,
        string RoomNumber,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId,
        DateTime BackInServiceAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
