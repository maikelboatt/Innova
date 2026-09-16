using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public sealed class RoomTypeDefinition:ValueObject
    {
        private RoomTypeDefinition( string name, MaxOccupancy maxOccupancy, Money baseRate )
        {
            Name = name;
            MaxOccupancy = maxOccupancy;
            BaseRate = baseRate;
        }

        public string Name { get; }
        public MaxOccupancy MaxOccupancy { get; }
        public Money BaseRate { get; }

        public static RoomTypeDefinition Of( string name, MaxOccupancy maxOccupancy, Money baseRate )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Room type name is required.");

            return new RoomTypeDefinition(name, maxOccupancy, baseRate);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return MaxOccupancy;
            yield return BaseRate;
        }
    }
}
