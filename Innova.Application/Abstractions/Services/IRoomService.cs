using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IRoomService
    {
        Task<Room> CreateAsync( RoomNumber roomNumber,
                                Floor floor,
                                RoomTypeId roomTypeId,
                                CancellationToken ct = default );

        // No command calls these directly — they're the integration point
        // other bounded contexts call into once FrontDesk/Housekeeping exist
        // (check-in occupies, checkout dirties, a passed inspection vacates).
        Task<Room> OccupyAsync( RoomId roomId, CancellationToken ct = default );

        Task<Room> MarkDirtyAsync( RoomId roomId, CancellationToken ct = default );

        Task<Room> MarkVacantAsync( RoomId roomId, CancellationToken ct = default );

        Task<Room> TakeOutOfServiceAsync( RoomId roomId, CancellationToken ct = default );

        Task<Room> ReturnToServiceAsync( RoomId roomId, CancellationToken ct = default );

        Task<Room?> FindVacantRoomAsync( RoomTypeId roomTypeId, CancellationToken ct = default );
    }
}
