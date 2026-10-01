using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Domain.RoomInventory.Repositories
{
    public interface IRoomTypeDefinitionRepository
    {
        Task<RoomTypeDefinition?> GetByIdAsync( RoomTypeId id, CancellationToken ct = default );

        Task<bool> ExistsByNameAsync( string name, CancellationToken ct = default );

        Task SaveAsync( RoomTypeDefinition room, CancellationToken ct = default );

        Task UpdateAsync( RoomTypeDefinition room, CancellationToken ct = default );
    }
}
