using Innova.Domain.Common;
using Innova.Domain.RoomInventory.Events;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.RoomInventory.Aggregates
{
    // Promoted from a plain ValueObject: it had no Id, despite RoomTypeId
    // already being referenced by both Room and RoomTypeAllotment. Without
    // an identity, there was no way to actually create and retrieve "the
    // definition of a Deluxe King" as a persisted thing.
    public sealed class RoomTypeDefinition:AggregateRoot<RoomTypeId>
    {
        private RoomTypeDefinition()
        {
        }

        private RoomTypeDefinition( RoomTypeId roomTypeId,
                                    string name,
                                    MaxOccupancy maxOccupancy,
                                    Money baseRate ):base(roomTypeId)
        {
            Name = name;
            MaxOccupancy = maxOccupancy;
            BaseRate = baseRate;
        }

        public string Name { get; private set; } = null!;
        public MaxOccupancy MaxOccupancy { get; private set; } = null!;
        public Money BaseRate { get; private set; } = null!;

        public static RoomTypeDefinition Define( string name, MaxOccupancy maxOccupancy, Money baseRate )
        {
            ArgumentNullException.ThrowIfNull(maxOccupancy);
            ArgumentNullException.ThrowIfNull(baseRate);

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Room type name is required.");

            RoomTypeId roomTypeId = RoomTypeId.New();
            RoomTypeDefinition roomType = new(
                roomTypeId,
                name,
                maxOccupancy,
                baseRate);

            roomType.RaiseDomainEvent(
                new RoomTypeDefined(
                    roomTypeId.Value,
                    name,
                    maxOccupancy.Value,
                    baseRate.Amount,
                    baseRate.Currency));

            return roomType;
        }

        public static RoomTypeDefinition Reconstitute( RoomTypeId roomTypeId,
                                                       string name,
                                                       MaxOccupancy maxOccupancy,
                                                       Money baseRate ) => new(
            roomTypeId,
            name,
            maxOccupancy,
            baseRate);
    }
}
