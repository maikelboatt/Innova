using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record ReservationMarkedNoShow(
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
        DateTime MarkedNoShowAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => ReservationId;
        string IAuditableEvent.EntityType => "Reservation";
        string IAuditableEvent.Summary => $"Reservation {ReservationId} marked no-show for Stay {StayStart:d} {GuestId} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
