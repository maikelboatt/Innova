using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Reservations.ValueObjects
{
    public sealed class GroupBookingId:ValueObject
    {
        private GroupBookingId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static GroupBookingId New() => new(Guid.NewGuid());

        public static GroupBookingId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("Group-Booking Id cannot be empty");

            return new GroupBookingId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
