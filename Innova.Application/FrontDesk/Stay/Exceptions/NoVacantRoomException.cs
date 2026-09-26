using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.FrontDesk.Stay.Exceptions
{
    public sealed class NoVacantRoomException( RoomTypeId roomTypeRequested ):ApplicationException($"No vacant room of type '{roomTypeRequested}' was found");
}
