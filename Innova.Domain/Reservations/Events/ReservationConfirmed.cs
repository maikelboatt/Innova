using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record ReservationConfirmed(
        Guid ReservationId,
        Guid GuestId,
        Guid? GroupBookingId,
        Guid RoomTypeRequestedId,
        DateOnly StayStart,
        DateOnly StayEnd,
        decimal RateAmount,
        string RateCurrency,
        DateTime ConfirmedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => ReservationId;
        string IAuditableEvent.EntityType => "Reservation";
        string IAuditableEvent.Summary => $"Reservation {ReservationId} confirmed for GuestId {GuestId} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
