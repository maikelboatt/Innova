using Innova.Application.Abstractions.Services;
using Innova.Application.Reservations.GroupBooking.Exceptions;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.Aggregates;
using Innova.Domain.Reservations.Repositories;
using Innova.Domain.Reservations.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class GroupBookingService( IGroupBookingRepository groupBookingRepository ):IGroupBookingService
    {
        public async Task<GroupBooking> OpenAsync( GuestId organizerGuestId, string groupName, CancellationToken ct = default )
        {
            GroupBooking groupBooking = GroupBooking.Open(organizerGuestId, groupName);

            await groupBookingRepository.SaveAsync(groupBooking, ct);

            return groupBooking;
        }

        public async Task<GroupBooking> AttachReservationAsync( GroupBookingId groupBookingId,
                                                                ReservationId reservationId,
                                                                CancellationToken ct = default )
        {
            GroupBooking groupBooking = await GetByIdAsync(groupBookingId, ct);

            groupBooking.AttachReservation(reservationId);

            await groupBookingRepository.UpdateAsync(groupBooking, ct);

            return groupBooking;
        }

        public async Task<GroupBooking> DetachReservationAsync( GroupBookingId groupBookingId,
                                                                ReservationId reservationId,
                                                                CancellationToken ct = default )
        {
            GroupBooking groupBooking = await GetByIdAsync(groupBookingId, ct);

            groupBooking.DetachReservation(reservationId);

            await groupBookingRepository.UpdateAsync(groupBooking, ct);

            return groupBooking;
        }

        private async Task<GroupBooking> GetByIdAsync( GroupBookingId groupBookingId, CancellationToken ct ) =>
            await groupBookingRepository.GetByIdAsync(groupBookingId, ct) ?? throw new GroupBookingNotFoundException(groupBookingId);
    }
}
