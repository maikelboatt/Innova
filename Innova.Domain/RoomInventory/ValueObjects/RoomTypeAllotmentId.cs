using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public class RoomTypeAllotmentId:ValueObject
    {
        private RoomTypeAllotmentId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static RoomTypeAllotmentId New() => new(Guid.NewGuid());

        public static RoomTypeAllotmentId From( Guid value ) => value == Guid.Empty
                                                                    ? throw new DomainException("RoomTypeAllotmentId cannot be empty.")
                                                                    : new RoomTypeAllotmentId(value);

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
