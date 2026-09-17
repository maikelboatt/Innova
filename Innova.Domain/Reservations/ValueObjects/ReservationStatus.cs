using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Reservations.ValueObjects
{
    public sealed class ReservationStatus:ValueObject
    {
        public static readonly ReservationStatus Tentative = new("Tentative");
        public static readonly ReservationStatus Confirmed = new("Confirmed");
        public static readonly ReservationStatus CheckedIn = new("CheckedIn");
        public static readonly ReservationStatus CheckedOut = new("CheckedOut");
        public static readonly ReservationStatus Cancelled = new("Cancelled");
        public static readonly ReservationStatus NoShow = new("NoShow");

        private ReservationStatus( string value ) => Value = value;


        public string Value { get; }

        public static ReservationStatus FromValue( string value ) => value switch
                                                                     {
                                                                         "Tentative" => Tentative,
                                                                         "Confirmed" => Confirmed,
                                                                         "CheckedIn" => CheckedIn,
                                                                         "CheckedOut" => CheckedOut,
                                                                         "Cancelled" => Cancelled,
                                                                         "NoShow" => NoShow,
                                                                         _ => throw new DomainException($"'{value}' is not a valid reservation status.")
                                                                     };

        public bool CanTransitionTo( ReservationStatus next )
        {
            return Value switch
                   {
                       "Tentative" => next == Confirmed
                                      || next == Cancelled,

                       "Confirmed" => next == CheckedIn
                                      || next == Cancelled || next == NoShow,

                       "CheckedIn" => next == CheckedOut,

                       _ => false
                   };
        }

        public override string ToString() => Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
