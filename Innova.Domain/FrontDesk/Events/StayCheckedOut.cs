using Innova.Domain.Common;

namespace Innova.Domain.FrontDesk.Events
{
    public sealed record StayCheckedOut(
        Guid StayId,
        Guid ReservationId,
        Guid RoomId,
        string RoomNumber,
        int MaxOccupancy,
        Guid? GroupBookingId,
        DateTime CheckedOut ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
