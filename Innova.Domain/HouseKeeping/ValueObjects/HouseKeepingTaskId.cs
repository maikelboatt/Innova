using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.HouseKeeping.ValueObjects
{
    public sealed class HouseKeepingTaskId:ValueObject
    {
        private HouseKeepingTaskId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static HouseKeepingTaskId New() => new(Guid.NewGuid());

        public static HouseKeepingTaskId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("HouseKeepingTaskId cannot be empty.");

            return new HouseKeepingTaskId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
