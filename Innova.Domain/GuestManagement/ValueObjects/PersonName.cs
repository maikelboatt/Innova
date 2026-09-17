using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.ValueObjects
{
    public sealed class PersonName:ValueObject
    {
        private PersonName( string firstName, string lastName, string? middleName = null )
        {
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            MiddleName = middleName?.Trim();
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string? MiddleName { get; }

        public static PersonName Create( string firstName, string lastName, string? middleName = null )
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new DomainException("First name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(lastName))
                throw new DomainException("Last name cannot be null or empty.");

            return new PersonName(firstName, lastName, middleName);
        }

        public override string ToString() => string.IsNullOrEmpty(MiddleName)
                                                 ? $"{FirstName} {LastName}"
                                                 : $"{FirstName} {MiddleName} {LastName}";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return FirstName;
            yield return LastName;
            yield return MiddleName;
        }
    }
}
