using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record ReservationCheckedIn(
        Guid ReservationId,
        Guid GuestId,
        Guid? GroupBookingId,
        Guid RoomTypeRequestedId,
        DateOnly StayStart,
        DateOnly StayEnd,
        decimal RateAmount,
        string RateCurrency,
        DateTime CheckedInAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => ReservationId;
        string IAuditableEvent.EntityType => "Reservation";
        string IAuditableEvent.Summary => $"GuestId {GuestId} checked in for Reservation {ReservationId} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
