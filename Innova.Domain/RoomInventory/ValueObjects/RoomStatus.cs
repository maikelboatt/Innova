using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public sealed class RoomStatus:ValueObject
    {
        public static readonly RoomStatus Vacant = new("Vacant");
        public static readonly RoomStatus Occupied = new("Occupied");
        public static readonly RoomStatus OutOfService = new("OutOfService");
        public static readonly RoomStatus Dirty = new("Dirty");

        private RoomStatus( string value ) => Value = value;

        public string Value { get; }

        public bool CanTransitionTo( RoomStatus next )
        {
            return Value switch
                   {
                       "Vacant" => next == Occupied
                                   || next == OutOfService,

                       "Occupied" => next == Dirty,

                       "OutOfService" => next == Dirty,

                       "Dirty" => next == Vacant,

                       _ => false
                   };
        }

        public static RoomStatus FromValue( string value )
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Room status is required");

            return new RoomStatus(value);
        }

        public override string ToString() => Value;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
