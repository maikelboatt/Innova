using Innova.Domain.Common;

namespace Innova.Domain.FrontDesk.Events
{
    public sealed record StayCheckedIn(
        Guid StayId,
        Guid ReservationId,
        Guid RoomId,
        string RoomNumber,
        int MaxOccupancy,
        Guid PrimaryOccupant,
        Guid? GroupBookingId,
        DateTime CheckedIn ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
