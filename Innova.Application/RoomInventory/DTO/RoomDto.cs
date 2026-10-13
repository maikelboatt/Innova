namespace Innova.Application.RoomInventory.DTO
{
    public record RoomDto(
        Guid RoomId,
        string RoomNumber,
        string RoomTypeName,
        int FloorLevel,
        string? FloorWing,
        Guid RoomTypeId,
        string Status );
}
