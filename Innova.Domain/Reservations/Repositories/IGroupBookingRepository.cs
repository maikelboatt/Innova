using Innova.Domain.Reservations.Aggregates;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Domain.Reservations.Repositories
{
    public interface IGroupBookingRepository
    {
        Task<GroupBooking?> GetByIdAsync( GroupBookingId id, CancellationToken ct = default );

        Task SaveAsync( GroupBooking groupBooking, CancellationToken ct = default );

        Task UpdateAsync( GroupBooking groupBooking, CancellationToken ct = default );
    }
}
