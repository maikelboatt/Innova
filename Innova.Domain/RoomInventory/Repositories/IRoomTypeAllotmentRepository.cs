using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Domain.RoomInventory.Repositories
{
    public interface IRoomTypeAllotmentRepository
    {
        Task<RoomTypeAllotment?> GetByIdAsync( RoomTypeAllotmentId id, CancellationToken ct = default );

        // A booking command knows a RoomTypeId and a stay period, not an
        // allotment id — it has to resolve which capacity bucket covers
        // that period before it can call Reserve()/Release() on it.
        Task<RoomTypeAllotment?> GetByRoomTypeAndPeriodAsync(
            RoomTypeId roomTypeId,
            DateOnly checkIn,
            DateOnly checkOut,
            CancellationToken ct = default );

        Task SaveAsync( RoomTypeAllotment allotment, CancellationToken ct = default );

        Task UpdateAsync( RoomTypeAllotment allotment, CancellationToken ct = default );
    }
}
