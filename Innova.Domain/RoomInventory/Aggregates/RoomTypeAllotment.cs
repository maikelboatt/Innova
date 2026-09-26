using Innova.Domain.Common;
using Innova.Domain.RoomInventory.Events;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.Aggregates
{
    public sealed class RoomTypeAllotment:AggregateRoot<RoomTypeAllotmentId>
    {
        private RoomTypeAllotment() { }

        private RoomTypeAllotment( RoomTypeAllotmentId id,
                                   RoomTypeId roomTypeId,
                                   DateOnly date,
                                   int totalRooms,
                                   int bookedCount ):base(id)
        {
            RoomTypeId = roomTypeId;
            Date = date;
            TotalRooms = totalRooms;
            BookedCount = bookedCount;
        }

        public RoomTypeId RoomTypeId { get; } = null!;
        public DateOnly Date { get; }
        public int TotalRooms { get; }
        public int BookedCount { get; private set; }

        public static RoomTypeAllotment Create( RoomTypeId roomTypeId, DateOnly date, int totalRooms )
        {
            if (totalRooms < 0)
                throw new DomainException("Total rooms cannot be negative.");

            RoomTypeAllotment allotment = new(
                RoomTypeAllotmentId.New(),
                roomTypeId,
                date,
                totalRooms,
                0);

            allotment.RaiseDomainEvent(
                new RoomTypeAllotmentCreated(
                    allotment.Id.Value,
                    roomTypeId.Value,
                    date,
                    totalRooms));

            return allotment;
        }

        public static RoomTypeAllotment Reconstitute( RoomTypeAllotmentId id,
                                                      RoomTypeId roomTypeId,
                                                      DateOnly date,
                                                      int totalRooms,
                                                      int bookedCount ) => new(
            id,
            roomTypeId,
            date,
            totalRooms,
            bookedCount);

        public void Reserve( int rooms = 1 )
        {
            if (rooms < 1)
                throw new DomainException("Must reserve at least one room.");

            if (BookedCount + rooms > TotalRooms)
                throw new DomainException($"Insufficient allotment for {RoomTypeId} on {Date}: {BookedCount}/{TotalRooms} already booked.");

            BookedCount += rooms;

            RaiseDomainEvent(
                new RoomTypeAllotmentReserved(
                    Id.Value,
                    RoomTypeId.Value,
                    Date,
                    rooms));
        }

        public void Release( int rooms = 1 )
        {
            if (BookedCount - rooms < 0)
                throw new DomainException("Cannot release more rooms than are currently booked.");

            BookedCount -= rooms;

            RaiseDomainEvent(
                new RoomTypeAllotmentReleased(
                    Id.Value,
                    RoomTypeId.Value,
                    Date,
                    rooms));
        }
    }
}
