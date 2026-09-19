using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Domain.RoomInventory.Repositories
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync( RoomId id, CancellationToken ct = default );

        // Front desk check-in needs "a Vacant room of this type" — the
        // result is loaded specifically to call Room.Occupy() on it, so
        // this belongs on the repository (it feeds a mutation), not a
        // read-model query.
        Task<Room?> FindVacantRoomAsync( RoomTypeId roomTypeId, CancellationToken ct = default );

        Task SaveAsync( Room room, CancellationToken ct = default );

        Task UpdateAsync( Room room, CancellationToken ct = default );
    }
}
