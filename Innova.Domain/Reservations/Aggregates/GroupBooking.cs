using Innova.Domain.Common;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.Events;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Reservations.Aggregates
{
    public sealed class GroupBooking:AggregateRoot<GroupBookingId>
    {
        private readonly List<ReservationId> _reservationIds = [];

        private GroupBooking()
        {
        }

        private GroupBooking( GroupBookingId groupBookingId, GuestId organizerGuestId, string groupName ):base(groupBookingId)
        {
            OrganizerGuestId = organizerGuestId;
            GroupName = groupName;
        }

        public GuestId OrganizerGuestId { get; }
        public string GroupName { get; set; }
        public IReadOnlyCollection<ReservationId> ReservationIds => _reservationIds.AsReadOnly();

        public static GroupBooking Open( GuestId organizerGuestId, string groupName )
        {
            ArgumentNullException.ThrowIfNull(organizerGuestId);
            if (string.IsNullOrWhiteSpace(groupName))
                throw new DomainException("A group booking requires a name.");

            GroupBookingId bookingId = GroupBookingId.New();

            GroupBooking groupBooking = new(bookingId, organizerGuestId, groupName);

            groupBooking.RaiseDomainEvent(new GroupBookingOpened(bookingId.Value, organizerGuestId.Value, groupName));

            return groupBooking;
        }

        public static GroupBooking Reconstitute( GroupBookingId groupBookingId, GuestId organizerGuestId, string groupName ) =>
            new(groupBookingId, organizerGuestId, groupName);

        public void DetachReservation( ReservationId reservationId )
        {
            if (!_reservationIds.Remove(reservationId))
                throw new DomainException("This reservation is not part of the group booking");

            RaiseDomainEvent(
                new GroupBookingReservationDetached(
                    Id.Value,
                    OrganizerGuestId.Value,
                    GroupName,
                    DateTime.UtcNow));
        }

        public void AttachReservation( ReservationId reservationId )
        {
            if (_reservationIds.Contains(reservationId))
                throw new DomainException("This reservation is already part of the group booking.");

            _reservationIds.Add(reservationId);

            RaiseDomainEvent(
                new GroupBookingReservationAttached(
                    Id.Value,
                    OrganizerGuestId.Value,
                    GroupName,
                    DateTime.UtcNow));
        }
    }
}
