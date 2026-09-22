using Innova.Application.Abstractions.Services;
using Innova.Application.RoomInventory.RoomTypeDefinition.Exceptions;
using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.Repositories;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class RoomTypeService( IRoomTypeDefinitionRepository roomTypeDefinitionRepository ):IRoomTypeService
    {
        public async Task<RoomTypeDefinition> DefineAsync( string name,
                                                           MaxOccupancy maxOccupancy,
                                                           Money baseRate,
                                                           CancellationToken ct = default )
        {
            RoomTypeDefinition roomTypeDefinition = RoomTypeDefinition.Define(name, maxOccupancy, baseRate);

            await roomTypeDefinitionRepository.SaveAsync(roomTypeDefinition, ct);

            return roomTypeDefinition;
        }

        public async Task<RoomTypeDefinition> GetByIdAsync( RoomTypeId roomTypeId, CancellationToken ct = default ) =>
            await roomTypeDefinitionRepository.GetByIdAsync(roomTypeId, ct) ?? throw new RoomTypeDefinitionNotFoundException(roomTypeId);
    }
}
