using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.ValueObjects
{
    public sealed class ContactDetails:ValueObject
    {
        private ContactDetails( string phoneNumber, string? email = null )
        {
            Email = email?.Trim();
            PhoneNumber = phoneNumber.Trim();
        }

        public string? Email { get; }
        public string PhoneNumber { get; }

        public static ContactDetails Create( string phoneNumber, string address, string? email = null )
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new DomainException("Phone number is required.");

            return new ContactDetails(phoneNumber, email);
        }

        public override string ToString() => $"Email: {Email}, Phone: {PhoneNumber}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Email;
            yield return PhoneNumber;
        }
    }
}
