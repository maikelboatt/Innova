using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record ReservationMarkedNoShow(
        Guid ReservationId,
        Guid Guest,
        Guid? GroupBookingId,
        Guid RoomTypeRequestedId,
        DateOnly StayStart,
        DateOnly StayEnd,
        decimal RateAmount,
        string RateCurrency,
        decimal FeeAmount,
        string FeeCurrency,
        DateTime MarkedNoShowAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
