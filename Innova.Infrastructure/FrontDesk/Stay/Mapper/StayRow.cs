namespace Innova.Infrastructure.FrontDesk.Stay.Mapper
{
    public sealed class StayRow
    {
        public Guid Id { get; init; }
        public Guid ReservationId { get; init; }
        public Guid AssignedRoomId { get; init; }
        public string AssignedRoomNumber { get; init; } = string.Empty;
        public Guid? GroupBookingId { get; init; }
        public int MaxOccupancy { get; init; }
        public DateTime? ActualCheckIn { get; init; }
        public DateTime? ActualCheckOut { get; init; }
    }
}
