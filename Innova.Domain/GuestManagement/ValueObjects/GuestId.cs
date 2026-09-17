using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.ValueObjects
{
    public sealed class GuestId:ValueObject
    {
        private GuestId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static GuestId New() => new(Guid.NewGuid());

        public static GuestId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("GuestId cannot be empty.");

            return new GuestId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
