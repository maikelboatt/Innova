using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.Aggregates;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IReservationService
    {
        // Reserves capacity BEFORE the Reservation is ever persisted — if
        // capacity reservation throws (no allotment, or fully booked),
        // nothing has been saved and the booking aborts cleanly.
        Task<Reservation> BookAsync( GuestId guestId,
                                     RoomTypeId roomTypeRequested,
                                     DateRange stayPeriod,
                                     RatePlan ratePlan,
                                     GroupBookingId? groupBookingId,
                                     CancellationToken ct = default );

        Task<Reservation> ConfirmAsync( ReservationId reservationId, bool hasGuarantee, CancellationToken ct = default );

        Task<Reservation> MarkCheckedInAsync( ReservationId reservationId, CancellationToken ct = default );

        Task<Reservation> MarkCheckedOutAsync( ReservationId reservationId, CancellationToken ct = default );

        // Releases the held capacity for the cancelled stay. Does NOT post
        // a cancellation fee — that's a reaction to the ReservationCancelled
        // domain event this raises, handled separately by Billing, since the
        // fee itself is already computed and carried inside that event.
        Task<Reservation> CancelAsync( ReservationId reservationId, CancellationToken ct = default );

        // Deliberately does not release capacity
        Task<Reservation> MarkNoShowAsync( ReservationId reservationId, CancellationToken ct = default );

        Task<Reservation> GetByIdAsync( ReservationId reservationId, CancellationToken ct = default );
    }
}
