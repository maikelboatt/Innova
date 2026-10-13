namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Mapping
{
    public sealed class AllotmentNightRow
    {
        public Guid RoomTypeId { get; init; }

        public string RoomTypeName { get; init; } = string.Empty;
        public DateOnly Date { get; init; }

        public int TotalRooms { get; init; }

        public int BookedCount { get; init; }

        public int Remaining { get; init; }
    }
}
