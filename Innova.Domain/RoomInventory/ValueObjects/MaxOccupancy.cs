using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public sealed class MaxOccupancy:ValueObject
    {
        private MaxOccupancy( int value ) => Value = value;

        public int Value { get; }

        public static MaxOccupancy Of( int value )
        {
            if (value < 1)
                throw new DomainException("Room max occupancy should be at least 1");

            return new MaxOccupancy(value);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
