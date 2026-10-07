using Innova.Application.Exceptions;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Exceptions
{
    public sealed class RoomTypeDefinitionNotFoundException( RoomTypeId roomTypeDefinitionId )
        :ApplicationExceptions($"Room type '{roomTypeDefinitionId}' was not found.");
}
