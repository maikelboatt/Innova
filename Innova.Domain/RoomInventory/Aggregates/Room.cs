using Innova.Domain.Common;
using Innova.Domain.RoomInventory.Events;
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
        public Floor Floor { get; } = null!;
        public RoomTypeId RoomTypeId { get; } = null!;
        public RoomStatus Status { get; private set; }

        public static Room Create( RoomNumber roomNumber,
                                   Floor floor,
                                   RoomTypeId roomTypeId )
        {
            ArgumentNullException.ThrowIfNull(roomNumber);
            ArgumentNullException.ThrowIfNull(floor);
            ArgumentNullException.ThrowIfNull(roomTypeId);

            RoomId roomId = RoomId.New();

            Room room = new(
                roomId,
                roomNumber,
                floor,
                roomTypeId,
                RoomStatus.Vacant);

            room.RaiseDomainEvent(
                new RoomCreated(
                    roomId.Value,
                    roomNumber.Value,
                    floor.Level,
                    floor.Wing,
                    roomTypeId.Value));

            return room;
        }

        public void Occupy()
        {
            if (!Status.CanTransitionTo(RoomStatus.Occupied))
                throw new DomainException($"Room {Number} cannot be occupied from status {Status}.");

            Status = RoomStatus.Occupied;

            RaiseDomainEvent(
                new RoomOccupied(
                    Id.Value,
                    Number.Value,
                    Floor.Level,
                    Floor.Wing,
                    RoomTypeId.Value,
                    DateTime.UtcNow));
        }

        public void MarkDirty()
        {
            if (!Status.CanTransitionTo(RoomStatus.Dirty))
                throw new DomainException($"Room {Number} can only become Dirty from Occupied.");

            Status = RoomStatus.Dirty;

            RaiseDomainEvent(
                new RoomDirtied(
                    Id.Value,
                    Number.Value,
                    Floor.Level,
                    Floor.Wing,
                    RoomTypeId.Value,
                    DateTime.UtcNow));
        }

        public void MarkVacant()
        {
            if (!Status.CanTransitionTo(RoomStatus.Vacant))
                throw new DomainException($"Room {Number} can only become Vacant from Dirty — housekeeping sign-off is required.");

            Status = RoomStatus.Vacant;

            RaiseDomainEvent(
                new RoomVacant(
                    Id.Value,
                    Number.Value,
                    Floor.Level,
                    Floor.Wing,
                    RoomTypeId.Value,
                    DateTime.UtcNow));
        }

        public void TakeOutOfService()
        {
            if (!Status.CanTransitionTo(RoomStatus.OutOfService))
                throw new DomainException($"Room {Number} cannot be taken out of service while Occupied.");

            Status = RoomStatus.OutOfService;

            RaiseDomainEvent(
                new RoomOutOfService(
                    Id.Value,
                    Number.Value,
                    Floor.Level,
                    Floor.Wing,
                    RoomTypeId.Value,
                    DateTime.UtcNow));
        }

        public void ReturnToService()
        {
            if (Status.CanTransitionTo(RoomStatus.Dirty))
                throw new DomainException($"Room {Number} is not currently OutOfService.");

            Status = RoomStatus.Dirty;

            RaiseDomainEvent(
                new RoomBackInService(
                    Id.Value,
                    Number.Value,
                    Floor.Level,
                    Floor.Wing,
                    RoomTypeId.Value,
                    DateTime.UtcNow));
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
