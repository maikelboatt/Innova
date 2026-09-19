using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.Events
{
    public sealed record RoomTypeDefined(
        Guid Value,
        string Name,
        int MaxOccupancyValue,
        decimal BaseRateAmount,
        string BaseRateCurrency ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
