namespace Innova.Application.RoomInventory.DTO
{
    public sealed record AllotmentNightDto(
        Guid RoomTypeId,
        string RoomTypeName,
        DateOnly Date,
        int TotalRooms,
        int BookedCount,
        int Remaining );
}
