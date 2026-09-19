using Innova.Domain.Common;

namespace Innova.Domain.Identity.ValueObjects
{
    public class Role:ValueObject
    {
        public static readonly Role Admin = new("Admin");
        public static readonly Role Receptionist = new("Receptionist");
        public static readonly Role Doctor = new("Doctor");
        public static readonly Role Pharmacist = new("Pharmacist");
        public static readonly Role BillingStaff = new("BillingStaff");

        private Role( string value )
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(value)} cannot be empty", nameof(value));

            Value = value;
        }

        public string Value { get; }

        public static Role From( string value ) => value switch
                                                   {
                                                       "Admin"        => Admin,
                                                       "Receptionist" => Receptionist,
                                                       "Doctor"       => Doctor,
                                                       "Pharmacist"   => Pharmacist,
                                                       "BillingStaff" => BillingStaff,
                                                       _              => throw new ArgumentException($"Invalid role '{value}'.")
                                                   };

        public override string ToString() => Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
