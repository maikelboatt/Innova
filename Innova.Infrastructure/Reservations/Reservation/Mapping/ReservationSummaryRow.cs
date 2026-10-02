namespace Innova.Infrastructure.Reservations.Reservation.Mapping
{
    public sealed class ReservationSummaryRow
    {
        public Guid Id { get; init; }

        public Guid GuestId { get; init; }

        public DateOnly StayStart { get; init; }

        public DateOnly StayEnd { get; init; }

        public string Status { get; init; } = string.Empty;
    }
}
