using Innova.Domain.Common;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.Aggregates
{
    public sealed class Room:AggregateRoot<RoomId>
    {
        private Room() { }

        private Room( RoomId roomId,
                      RoomNumber roomNumber,
                      Floor floor,
                      RoomTypeId roomTypeId,
                      RoomStatus roomStatus ):base(roomId)
        {
            Number = roomNumber;
            Floor = floor;
            RoomTypeId = roomTypeId;
            Status = roomStatus;
        }

        public RoomNumber Number { get; } = null!;
        public Floor Floor { get; private set; } = null!;
        public RoomTypeId RoomTypeId { get; private set; } = null!;
        public RoomStatus Status { get; private set; }

        public static Room Create( RoomNumber roomNumber,
                                   Floor floor,
                                   RoomTypeId roomTypeId,
                                   RoomStatus roomStatus )
        {
            ArgumentNullException.ThrowIfNull(roomNumber);
            ArgumentNullException.ThrowIfNull(floor);
            ArgumentNullException.ThrowIfNull(roomTypeId);
            ArgumentNullException.ThrowIfNull(roomStatus);

            RoomId roomId = RoomId.New();

            Room room = new(
                roomId,
                roomNumber,
                floor,
                roomTypeId,
                RoomStatus.Vacant);

            // Raise Domain Event

            return room;
        }

        public void Occupy()
        {
            if (!Status.CanTransitionTo(RoomStatus.Occupied))
                throw new DomainException($"Room {Number} cannot be occupied from status {Status}.");

            Status = RoomStatus.Occupied;
        }

        public void MarkDirty()
        {
            if (!Status.CanTransitionTo(RoomStatus.Dirty))
                throw new DomainException($"Room {Number} cannot be occupied from status {Status}.");

            Status = RoomStatus.Dirty;
        }

        public void MarkVacant()
        {
            if (!Status.CanTransitionTo(RoomStatus.Vacant))
                throw new DomainException($"Room {Number} cannot be occupied from status {Status}.");

            Status = RoomStatus.Vacant;
        }

        public void TakeOutOfService()
        {
            if (!Status.CanTransitionTo(RoomStatus.OutOfService))
                throw new DomainException($"Room {Number} cannot be occupied from status {Status}.");

            Status = RoomStatus.OutOfService;
        }

        public void ReturnToService()
        {
            if (Status != RoomStatus.OutOfService)
                throw new DomainException($"Room {Number} cannot be occupied from status {Status}.");

            Status = RoomStatus.Dirty;
        }

        public static Room Reconstitute( RoomId roomId,
                                         RoomNumber roomNumber,
                                         Floor floor,
                                         RoomTypeId roomTypeId,
                                         RoomStatus roomStatus )
        {
            Room room = new(
                roomId,
                roomNumber,
                floor,
                roomTypeId,
                roomStatus);

            return room;
        }
    }
}
