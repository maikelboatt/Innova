using Innova.Application.Exceptions;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Exceptions
{
    public sealed class RoomNotFoundException( RoomId roomId ):ApplicationExceptions($"Room '{roomId}' was not found.");
}
