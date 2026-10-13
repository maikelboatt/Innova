using Innova.Application.Exceptions;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Exceptions
{
    public class DuplicateRoomTypeNameException( string roomName ):ApplicationExceptions(
        $"Room type with name '{roomName}' already exists.");
}
