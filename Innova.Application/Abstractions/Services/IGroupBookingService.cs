using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.Aggregates;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IGroupBookingService
    {
        Task<GroupBooking> OpenAsync( GuestId organizerGuestId, string groupName, CancellationToken ct = default );

        Task<GroupBooking> AttachReservationAsync( GroupBookingId groupBookingId,
                                                   ReservationId reservationId,
                                                   CancellationToken ct = default );

        Task<GroupBooking> DetachReservationAsync( GroupBookingId groupBookingId,
                                                   ReservationId reservationId,
                                                   CancellationToken ct = default );
    }
}
