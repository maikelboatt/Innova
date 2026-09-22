using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Exceptions
{
    public sealed class RoomNotFoundException( RoomId roomId ):ApplicationException($"Room '{roomId}' was not found.");
}
