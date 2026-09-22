using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IRoomTypeService
    {
        Task<RoomTypeDefinition> DefineAsync( string name,
                                              MaxOccupancy maxOccupancy,
                                              Money baseRate,
                                              CancellationToken ct = default );

        Task<RoomTypeDefinition> GetByIdAsync( RoomTypeId roomTypeId, CancellationToken ct = default );
    }
}
