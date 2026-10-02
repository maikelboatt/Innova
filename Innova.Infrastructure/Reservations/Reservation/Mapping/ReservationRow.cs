namespace Innova.Infrastructure.Reservations.Reservation.Mapping
{
    public sealed class ReservationRow
    {
        public Guid Id { get; init; }

        public Guid GuestId { get; init; }

        public Guid? GroupBookingId { get; init; }

        public Guid RoomTypeRequestedId { get; init; }

        public DateOnly StayStart { get; init; }

        public DateOnly StayEnd { get; init; }

        public decimal NightlyRateAmount { get; init; }

        public string NightlyRateCurrency { get; init; } = string.Empty;

        public int FreeCancellationWindowHrs { get; init; }

        public decimal CancellationFeeAmount { get; init; }

        public string CancellationFeeCurrency { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;
    }
}
