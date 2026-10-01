namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Mapping
{
    public sealed class AllotmentNightRow
    {
        public DateOnly Date { get; init; }

        public int TotalRooms { get; init; }

        public int BookedCount { get; init; }

        public int Remaining { get; init; }
    }
}
