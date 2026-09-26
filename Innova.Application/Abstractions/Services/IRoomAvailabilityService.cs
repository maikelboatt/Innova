using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    // Owns the mutating, invariant-enforcing side of capacity — reserving
    // and releasing. A plain "is there availability" check for display
    // purposes (browsing, search results) is a read and belongs in a query
    // handler that goes straight to SQL, not here; this service exists
    // specifically for the write path that has to be transactionally safe.
    public interface IRoomAvailabilityService
    {
        Task<IReadOnlyCollection<RoomTypeAllotment>> CreateAsync( RoomTypeId roomTypeId,
                                                                  DateRange date,
                                                                  int totalRooms,
                                                                  CancellationToken ct );

        Task<IReadOnlyCollection<RoomTypeAllotment>> ReserveCapacityAsync( RoomTypeId roomTypeId,
                                                                           DateRange date,
                                                                           int rooms,
                                                                           CancellationToken ct = default );

        Task<IReadOnlyCollection<RoomTypeAllotment>> ReleaseCapacityAsync( RoomTypeId roomTypeId,
                                                                           DateRange stayPeriod,
                                                                           int rooms,
                                                                           CancellationToken ct = default );
    }
}
