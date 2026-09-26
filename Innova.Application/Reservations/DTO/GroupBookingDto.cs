namespace Innova.Application.Reservations.DTO
{
    public sealed record GroupBookingDto(
        Guid GroupBookingId,
        Guid OrganizerGuestId,
        string GroupName,
        IReadOnlyCollection<Guid> ReservationIds );
}
