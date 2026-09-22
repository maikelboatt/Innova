using Innova.Application.Abstractions.Services;
using Innova.Application.Reservations.Reservation.Exceptions;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.Aggregates;
using Innova.Domain.Reservations.Repositories;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class ReservationService( IReservationRepository reservationRepository, IRoomAvailabilityService roomAvailabilityService ):IReservationService
    {
        public async Task<Reservation> BookAsync( GuestId guestId,
                                                  RoomTypeId roomTypeRequested,
                                                  DateRange stayPeriod,
                                                  RatePlan ratePlan,
                                                  GroupBookingId? groupBookingId,
                                                  CancellationToken ct = default )
        {
            Reservation reservation = Reservation.Booked(
                guestId,
                roomTypeRequested,
                stayPeriod,
                ratePlan,
                groupBookingId);

            await roomAvailabilityService.ReserveCapacityAsync(
                roomTypeRequested,
                stayPeriod,
                1,
                ct);

            await reservationRepository.SaveAsync(reservation, ct);

            return reservation;
        }

        public async Task<Reservation> ConfirmAsync( ReservationId reservationId, bool hasGuarantee, CancellationToken ct = default )
        {
            Reservation reservation = await GetByIdAsync(reservationId, ct);

            reservation.Confirm(hasGuarantee);

            await reservationRepository.UpdateAsync(reservation, ct);

            return reservation;
        }

        public async Task<Reservation> MarkCheckedInAsync( ReservationId reservationId, CancellationToken ct = default )
        {
            Reservation reservation = await GetByIdAsync(reservationId, ct);

            reservation.MarkCheckedIn();

            await reservationRepository.UpdateAsync(reservation, ct);

            return reservation;
        }

        public async Task<Reservation> MarkCheckedOutAsync( ReservationId reservationId, CancellationToken ct = default )
        {
            Reservation reservation = await GetByIdAsync(reservationId, ct);

            reservation.MarkCheckedOut();

            await reservationRepository.UpdateAsync(reservation, ct);

            return reservation;
        }

        public async Task<Reservation> CancelAsync( ReservationId reservationId, CancellationToken ct = default )
        {
            Reservation reservation = await GetByIdAsync(reservationId, ct);

            reservation.MarkCheckedIn();

            await reservationRepository.UpdateAsync(reservation, ct);

            await roomAvailabilityService.ReleaseCapacityAsync(
                reservation.RoomTypeRequested,
                reservation.StayPeriod,
                1,
                ct);

            return reservation;
        }

        public async Task<Reservation> MarkNoShowAsync( ReservationId reservationId, CancellationToken ct = default )
        {
            Reservation reservation = await GetByIdAsync(reservationId, ct);

            reservation.MarkNoShow();

            await reservationRepository.UpdateAsync(reservation, ct);

            return reservation;
        }

        public async Task<Reservation> GetByIdAsync( ReservationId reservationId, CancellationToken ct = default ) =>
            await reservationRepository.GetByIdAsync(reservationId, ct) ?? throw new ReservationNotFoundException(reservationId);
    }
}
