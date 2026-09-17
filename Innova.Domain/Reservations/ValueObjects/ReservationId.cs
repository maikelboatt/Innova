using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Reservations.ValueObjects
{
    public sealed class ReservationId:ValueObject
    {
        private ReservationId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static ReservationId New() => new(Guid.NewGuid());

        public static ReservationId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("Reservation Id cannot be empty");

            return new ReservationId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
