namespace Innova.Application.Reservations.DTO
{
    public sealed record ReservationSummaryDto(
        Guid ReservationId,
        Guid GuestId,
        DateOnly CheckIn,
        DateOnly CheckOut,
        string Status );
}
