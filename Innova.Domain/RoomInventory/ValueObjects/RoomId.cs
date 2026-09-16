using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public sealed class RoomId:ValueObject
    {
        private RoomId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static RoomId New() => new(Guid.NewGuid());

        public static RoomId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("RoomId cannot be empty.");

            return new RoomId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
