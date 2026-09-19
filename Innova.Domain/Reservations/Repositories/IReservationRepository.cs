using Innova.Domain.Reservations.Aggregates;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Domain.Reservations.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync( ReservationId reservationId, CancellationToken ct = default );

        Task SaveAsync( Reservation reservation, CancellationToken ct = default );

        Task UpdateAsync( Reservation reservation, CancellationToken ct = default );
    }
}
