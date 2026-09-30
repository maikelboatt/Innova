using Innova.Domain.Common;

namespace Innova.Domain.Reservations.Events
{
    public sealed record ReservationBooked(
        Guid ReservationId,
        Guid GuestId,
        Guid? GroupBookingId,
        Guid RoomTypeRequestedId,
        DateOnly StayStart,
        DateOnly StayEnd,
        decimal RateAmount,
        string RateCurrency ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => ReservationId;
        string IAuditableEvent.EntityType => "Reservation";
        string IAuditableEvent.Summary => $"Reservation {ReservationId} Booked for GuestId {GuestId} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
