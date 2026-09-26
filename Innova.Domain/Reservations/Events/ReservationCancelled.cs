using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record ReservationCancelled(
        Guid ReservationId,
        Guid GuestId,
        Guid? GroupBookingId,
        Guid RoomTypeRequestedId,
        DateOnly StayStart,
        DateOnly StayEnd,
        decimal RateAmount,
        string RateCurrency,
        decimal FeeAmount,
        string FeeCurrency,
        DateTime CancelledAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
