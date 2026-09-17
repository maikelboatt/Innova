using Innova.Domain.Common;
using Innova.Domain.FrontDesk.Events;
using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.FrontDesk.Aggregates
{
    public sealed class Stay:AggregateRoot<StayId>
    {
        private readonly List<GuestId> _occupants = [];

        private Stay()
        {

        }

        private Stay( StayId stayId,
                      ReservationId reservationId,
                      RoomId roomId,
                      RoomNumber roomNumber,
                      MaxOccupancy maxOccupancySnapshot,
                      DateTime? actualCheckIn,
                      DateTime? actualCheckOut,
                      GroupBookingId? groupBookingId = null ):base(stayId)
        {
            ReservationId = reservationId;
            AssignedRoomId = roomId;
            AssignedRoomNumber = roomNumber;
            MaxOccupancy = maxOccupancySnapshot;
            ActualCheckIn = actualCheckIn;
            ActualCheckOut = actualCheckOut;
            GroupBookingId = groupBookingId;
        }

        public ReservationId ReservationId { get; }
        public GroupBookingId? GroupBookingId { get; }
        public RoomId AssignedRoomId { get; }
        public RoomNumber AssignedRoomNumber { get; }

        public MaxOccupancy MaxOccupancy { get; } = null!;

        public DateTime? ActualCheckIn { get; }
        public DateTime? ActualCheckOut { get; private set; }
        public IReadOnlyCollection<GuestId> Occupants => _occupants.AsReadOnly();

        public static Stay CheckIn( ReservationId reservationId,
                                    RoomId roomId,
                                    RoomNumber roomNumber,
                                    MaxOccupancy maxOccupancySnapshot,
                                    GuestId primaryOccupant,
                                    GroupBookingId? groupBookingId = null )
        {
            ArgumentNullException.ThrowIfNull(reservationId);
            ArgumentNullException.ThrowIfNull(roomId);
            ArgumentNullException.ThrowIfNull(roomNumber);
            ArgumentNullException.ThrowIfNull(maxOccupancySnapshot);
            ArgumentNullException.ThrowIfNull(primaryOccupant);

            StayId stayId = StayId.New();
            Stay stay = new(
                stayId,
                reservationId,
                roomId,
                roomNumber,
                maxOccupancySnapshot,
                DateTime.UtcNow,
                null,
                groupBookingId);

            stay._occupants.Add(primaryOccupant);

            stay.RaiseDomainEvent(
                new StayCheckedIn(
                    stayId.Value,
                    reservationId.Value,
                    roomId.Value,
                    roomNumber.Value,
                    maxOccupancySnapshot.Value,
                    primaryOccupant.Value,
                    groupBookingId?.Value,
                    DateTime.UtcNow));
            return stay;
        }

        public static Stay Reconstitute( StayId id,
                                         ReservationId reservationId,
                                         GroupBookingId? groupBookingId,
                                         RoomId roomId,
                                         RoomNumber roomNumber,
                                         MaxOccupancy maxOccupancy,
                                         DateTime? actualCheckIn,
                                         DateTime? actualCheckOut,
                                         IEnumerable<GuestId> occupants )
        {
            Stay stay = new(
                id,
                reservationId,
                roomId,
                roomNumber,
                maxOccupancy,
                actualCheckIn,
                actualCheckOut,
                groupBookingId);

            stay._occupants.AddRange(occupants);
            return stay;
        }

        public void AddOccupant( GuestId guestId )
        {
            if (ActualCheckOut is not null)
                throw new DomainException("Cannot add an occupant after checkout.");

            if (_occupants.Contains(guestId))
                throw new DomainException("This guest is already registered against the stay.");

            if (_occupants.Count >= MaxOccupancy.Value)
                throw new DomainException($"Room {AssignedRoomNumber} is at its maximum occupancy of {MaxOccupancy.Value}.");

            _occupants.Add(guestId);

            RaiseDomainEvent(
                new StayAddedOccupant(
                    Id.Value,
                    ReservationId.Value,
                    AssignedRoomId.Value,
                    AssignedRoomNumber.Value,
                    guestId.Value,
                    MaxOccupancy.Value,
                    DateTime.UtcNow));
        }

        public void CheckOut()
        {
            if (ActualCheckIn is null)
                throw new DomainException("Cannot check out a stay that was never checked in.");

            if (ActualCheckOut is not null)
                throw new DomainException("This stay has already been checked out.");

            // "Zero balance across every associated folio" is enforced entirely
            // by CheckOutCommandHandler BEFORE this method is called. Stay does
            // NOT track its own FolioIds — Folio.Owner is the single authoritative
            // record of that relationship (Stay tracking a mirrored list would be
            // the same fact stored twice, updatable by two separate transactions,
            // with no guarantee they stay in sync).
            //
            // The handler instead runs a query: all Folios owned directly by
            // this StayId, plus all Folios owned by any GuestId in Occupants
            // (split-billing folios), plus — if GroupBookingId is set — any
            // folio owned by the group booking. It sums their balances and
            // refuses to call CheckOut() at all if any is non-zero.
            ActualCheckOut = DateTime.UtcNow;

            RaiseDomainEvent(
                new StayCheckedOut(
                    Id.Value,
                    ReservationId.Value,
                    AssignedRoomId.Value,
                    AssignedRoomNumber.Value,
                    MaxOccupancy.Value,
                    GroupBookingId?.Value,
                    DateTime.UtcNow));
        }
    }
}
