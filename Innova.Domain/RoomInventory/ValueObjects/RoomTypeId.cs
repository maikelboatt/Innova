using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public sealed class RoomTypeId:ValueObject
    {
        private RoomTypeId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static RoomTypeId New() => new(Guid.NewGuid());

        public static RoomTypeId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("RoomTypeId cannot be empty.");

            return new RoomTypeId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
