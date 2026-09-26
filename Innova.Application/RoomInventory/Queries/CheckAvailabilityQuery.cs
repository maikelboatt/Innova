using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;

namespace Innova.Application.RoomInventory.Queries
{
    public sealed record CheckAvailabilityQuery( Guid RoomTypeId, DateOnly CheckIn, DateOnly CheckOut ):IQuery<AvailabilityDto>;
}
