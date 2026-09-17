using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.HouseKeeping.ValueObjects
{
    public sealed class StaffId:ValueObject
    {
        private StaffId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static StaffId New() => new(Guid.NewGuid());

        public static StaffId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("StaffId cannot be empty.");

            return new StaffId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
