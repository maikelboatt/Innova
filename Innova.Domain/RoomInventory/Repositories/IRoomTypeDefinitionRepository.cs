using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Domain.RoomInventory.Repositories
{
    public interface IRoomTypeDefinitionRepository
    {
        Task<RoomTypeDefinition?> GetByIdAsync( RoomTypeId id, CancellationToken ct = default );

        // Front desk check-in needs "a Vacant room of this type" — the
        // result is loaded specifically to call RoomTypeDefinition.Occupy() on it, so
        // this belongs on the repository (it feeds a mutation), not a
        // read-model query.
        Task<RoomTypeDefinition?> RoomTypeDefinitionAsync( RoomTypeId roomTypeId, CancellationToken ct = default );

        Task<bool> ExistByIdentityAsync( string name, int maxOccupancy, CancellationToken ct = default );

        Task SaveAsync( RoomTypeDefinition room, CancellationToken ct = default );

        Task UpdateAsync( RoomTypeDefinition room, CancellationToken ct = default );
    }
}
