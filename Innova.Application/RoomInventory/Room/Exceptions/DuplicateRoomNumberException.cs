using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Exceptions
{
    public class DuplicateRoomNumberException( RoomNumber roomNumber ):ApplicationException(
        $"Room with Number '{roomNumber.Value}' already exists");
}
