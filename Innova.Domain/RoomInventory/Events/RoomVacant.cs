using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomVacant(
        Guid RoomId,
        string RoomNumber,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId,
        DateTime VacantAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
