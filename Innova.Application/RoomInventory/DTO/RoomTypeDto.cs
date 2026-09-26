namespace Innova.Application.RoomInventory.DTO
{
    public record RoomTypeDto(
        Guid RoomTypeId,
        string Name,
        int MaxOccupancy,
        decimal BaseRateAmount,
        string BaseRateCurrency );
}
