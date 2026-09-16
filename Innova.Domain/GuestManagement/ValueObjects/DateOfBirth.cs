using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.ValueObjects
{
    public sealed class DateOfBirth:ValueObject
    {
        private DateOfBirth( DateOnly value ) => Value = value;

        public DateOnly Value { get; }

        public override string ToString() => Value.ToShortDateString();

        public static DateOfBirth Create( DateOnly value )
        {
            if (value > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new DomainException("Date of birth cannot be in the future.");

            return new DateOfBirth(value);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public int GetAge()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = today.Year - Value.Year;

            if (today < Value.AddYears(age))
                age--;

            return age;
        }
    }
}
