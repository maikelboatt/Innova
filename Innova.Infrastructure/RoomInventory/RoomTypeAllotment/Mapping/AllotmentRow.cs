namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Mapping
{
    public sealed class AllotmentRow
    {
        public Guid Id { get; init; }
        public Guid RoomTypeId { get; init; }
        public DateOnly Date { get; init; }
        public int TotalRooms { get; init; }
        public int BookedCount { get; init; }
    }
}
