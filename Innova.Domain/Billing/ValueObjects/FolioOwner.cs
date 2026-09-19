using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Billing.ValueObjects
{
    public sealed class FolioOwner:ValueObject
    {
        private FolioOwner( FolioOwnerType type, Guid ownerId )
        {
            if (ownerId == Guid.Empty)
                throw new DomainException("Folio owner is required");

            Type = type;
            OwnerId = ownerId;
        }

        public FolioOwnerType Type { get; }
        public Guid OwnerId { get; }

        public static FolioOwner ForStay( Guid stayId ) => new(FolioOwnerType.Stay, stayId);

        public static FolioOwner ForGuestWithinStay( Guid guestId ) => new(FolioOwnerType.GuestWithinStay, guestId);

        public static FolioOwner ForGroupBooking( Guid groupBookingId ) => new(FolioOwnerType.GroupBooking, groupBookingId);

        public override string ToString() => $"Owner Id: '{OwnerId}' Type: '{Type.ToString()}' ";

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Type;
            yield return OwnerId;
        }
    }
}
