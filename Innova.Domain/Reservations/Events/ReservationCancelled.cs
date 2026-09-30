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
        DateTime CancelledAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => ReservationId;
        string IAuditableEvent.EntityType => "Reservation";
        string IAuditableEvent.Summary => $"Cancelled at {CancelledAt:g}; fee of {FeeAmount} {FeeCurrency}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
