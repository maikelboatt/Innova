using Innova.Domain.Common;

namespace Innova.Domain.FrontDesk.Events
{
    public sealed record StayAddedOccupant(
        Guid StayId,
        Guid ReservationId,
        Guid RoomId,
        string RoomNumber,
        Guid GuestId,
        int MaxOccupancy,
        DateTime OccupantAddedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
