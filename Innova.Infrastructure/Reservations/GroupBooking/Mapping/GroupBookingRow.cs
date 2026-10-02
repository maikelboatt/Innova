namespace Innova.Infrastructure.Reservations.GroupBooking.Mapping
{
    public sealed class GroupBookingRow
    {
        public Guid Id { get; init; }
        public Guid OrganizerGuestId { get; init; }
        public string GroupName { get; init; } = string.Empty;
        public IReadOnlyCollection<Guid> ReservationIds { get; init; } = [];
    }
}
