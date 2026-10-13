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
            bool exists = await roomTypeDefinitionRepository.ExistsByNameAsync(name, ct);
            if (exists)
                throw new DuplicateRoomTypeNameException(name);

            RoomTypeDefinition roomTypeDefinition = RoomTypeDefinition.Define(name, maxOccupancy, baseRate);

            await roomTypeDefinitionRepository.SaveAsync(roomTypeDefinition, ct);

            return roomTypeDefinition;
        }

        public async Task<RoomTypeDefinition> GetByIdAsync( RoomTypeId roomTypeId, CancellationToken ct = default ) =>
            await roomTypeDefinitionRepository.GetByIdAsync(roomTypeId, ct) ?? throw new RoomTypeDefinitionNotFoundException(roomTypeId);

        public async Task UpdateAsync( RoomTypeId roomTypeId,
                                       string name,
                                       MaxOccupancy maxOccupancy,
                                       Money baseRate,
                                       CancellationToken ct = default )
        {
            RoomTypeDefinition roomTypeDefinition =
                await GetByIdAsync(roomTypeId, ct);

            bool nameExists =
                await roomTypeDefinitionRepository.ExistsByNameAsync(name, ct);

            if (nameExists &&
                !string.Equals(
                    roomTypeDefinition.Name,
                    name.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                throw new DuplicateRoomTypeNameException(name);

            roomTypeDefinition.UpdateDetails(
                name,
                maxOccupancy,
                baseRate);

            await roomTypeDefinitionRepository.UpdateAsync(
                roomTypeDefinition,
                ct);
        }
    }
}
