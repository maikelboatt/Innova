using Innova.Domain.FrontDesk.Aggregates;
using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IStayService
    {
        Task<Stay> CheckInAsync( ReservationId reservationId, GuestId primaryOccupant, CancellationToken ct = default );

        Task<Stay> CheckOutAsync( StayId stayId, CancellationToken ct = default );

        Task<Stay> AddOccupantAsync( StayId stayId, GuestId guestId, CancellationToken ct = default );

        Task<Stay> GetByIdAsync( StayId stayId, CancellationToken ct = default );
    }
}
