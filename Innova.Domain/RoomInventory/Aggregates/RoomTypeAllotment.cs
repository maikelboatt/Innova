using Innova.Domain.Common;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.RoomInventory.Aggregates
{
    public sealed class RoomTypeAllotment:AggregateRoot<RoomTypeAllotmentId>
    {
        private RoomTypeAllotment() { }

        private RoomTypeAllotment( RoomTypeAllotmentId roomTypeAllotmentId,
                                   RoomTypeId roomTypeId,
                                   DateRange period,
                                   int totalRooms ):base(roomTypeAllotmentId)
        {
            RoomTypeId = roomTypeId;
            Period = period;
            TotalRooms = totalRooms;
        }

        public RoomTypeId RoomTypeId { get; private set; } = null!;
        public DateRange Period { get; } = null!;
        public int TotalRooms { get; }
        public int BookedCount { get; private set; }

        public static RoomTypeAllotment Create( RoomTypeId roomTypeId, DateRange period, int totalRooms )
        {
            if (totalRooms < 0)
                throw new DomainException("Total rooms cannot be negative");

            RoomTypeAllotmentId roomTypeAllotmentId = RoomTypeAllotmentId.New();
            RoomTypeAllotment roomTypeAllotment = new(
                                                      roomTypeAllotmentId,
                                                      roomTypeId,
                                                      period,
                                                      totalRooms)
                                                  {
                                                      BookedCount = 0
                                                  };


            return roomTypeAllotment;
        }

        public static RoomTypeAllotment Reconstitute( RoomTypeAllotmentId id,
                                                      RoomTypeId roomTypeId,
                                                      DateRange period,
                                                      int totalRooms,
                                                      int bookedCount ) => new(
                                                                               id,
                                                                               roomTypeId,
                                                                               period,
                                                                               totalRooms)
                                                                           {
                                                                               BookedCount = bookedCount
                                                                           };

        public void Reserve( int rooms = 1 )
        {
            if (rooms < 1)
                throw new DomainException("Must reserve at least one room");

            if (BookedCount + rooms > TotalRooms)
                throw new DomainException($"Insufficient allotment for {Period.Start}–{Period.End}: {BookedCount}/{TotalRooms} already booked.");

            BookedCount += rooms;
        }

        public void Release( int rooms = 1 )
        {
            if (BookedCount - rooms < 0)
                throw new DomainException("Cannot release more rooms than are currently booked.");
            BookedCount -= rooms;
        }
    }
}
