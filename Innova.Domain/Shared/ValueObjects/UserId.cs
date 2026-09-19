using Innova.Domain.Common;

namespace Innova.Domain.Shared.ValueObjects
{
    public sealed class UserId:ValueObject
    {
        private UserId( Guid value )
        {
            if (value == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(value));

            Value = value;
        }

        public Guid Value { get; }

        public static UserId New() => new(Guid.NewGuid());

        public static UserId From( Guid value ) => new(value);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
