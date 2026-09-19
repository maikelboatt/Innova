using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Billing.ValueObjects
{
    public sealed class FolioId:ValueObject
    {
        private FolioId( Guid value ) => Value = value;

        public Guid Value { get; }

        public static FolioId New() => new(Guid.NewGuid());

        public static FolioId From( Guid value )
        {
            if (value == Guid.Empty)
                throw new DomainException("FolioId cannot be empty.");

            return new FolioId(value);
        }

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
