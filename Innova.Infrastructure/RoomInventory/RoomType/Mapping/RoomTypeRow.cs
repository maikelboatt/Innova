namespace Innova.Infrastructure.RoomInventory.RoomType.Mapping
{
    public sealed class RoomTypeRow
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public int MaxOccupancy { get; init; }
        public decimal BaseRateAmount { get; init; }
        public string BaseRateCurrency { get; init; } = string.Empty;
    }
}
