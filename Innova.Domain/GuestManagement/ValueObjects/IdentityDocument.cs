using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.ValueObjects
{
    public sealed class IdentityDocument:ValueObject
    {
        private IdentityDocument( IdentityDocumentType type, string number )
        {
            Type = type;
            Number = number;
        }

        public IdentityDocumentType Type { get; }
        public string Number { get; }

        public static IdentityDocument Create( IdentityDocumentType type, string number )
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new DomainException("Identity document number is required.");

            string normalizedNumber = number
                                      .Trim()
                                      .ToUpperInvariant();

            if (normalizedNumber.Length > 50)
                throw new DomainException(
                    "Identity document number cannot exceed 50 characters.");

            return new IdentityDocument(type, normalizedNumber);
        }

        public override string ToString() => $"{Type}: {Number}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Type;
            yield return Number;
        }
    }
}
