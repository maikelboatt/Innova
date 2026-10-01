namespace Innova.Infrastructure.RoomInventory.Room.Mapping
{
    public sealed class RoomRow
    {
        public Guid Id { get; init; }
        public string RoomNumber { get; init; } = string.Empty;
        public int FloorLevel { get; init; }
        public string? FloorWing { get; init; }
        public Guid RoomTypeId { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
