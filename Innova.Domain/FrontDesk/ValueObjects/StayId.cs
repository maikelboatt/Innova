using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.FrontDesk.ValueObjects
{
    public sealed class StayId:ValueObject
    {
        private StayId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static StayId New() => new(Guid.NewGuid());

        public static StayId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("Stay Id cannot be empty");

            return new StayId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
