using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public sealed class RoomNumber:ValueObject
    {
        private RoomNumber( string value ) => Value = value;

        public string Value { get; }

        public static RoomNumber Of( string value )
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Room number is required");

            return new RoomNumber(value);
        }

        public override string ToString() => Value.Trim();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
