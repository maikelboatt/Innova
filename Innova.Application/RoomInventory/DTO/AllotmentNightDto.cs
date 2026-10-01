namespace Innova.Application.RoomInventory.DTO
{
    public sealed record AllotmentNightDto(
        DateOnly Date,
        int TotalRooms,
        int BookedCount,
        int Remaining );
}
