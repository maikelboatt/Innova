namespace Innova.Application.Reservations.DTO
{
    public sealed record ReservationDto(
        Guid ReservationId,
        Guid GuestId,
        Guid? GroupBookingId,
        Guid RootTypeRequested,
        DateOnly CheckIn,
        DateOnly CheckOut,
        decimal NightlyRateAmount,
        string NightlyRateCurrency,
        string Status );
}
