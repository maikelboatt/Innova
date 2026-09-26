namespace Innova.Application.FrontDesk.Stay.DTO
{
    public sealed record StayDto(
        Guid StayId,
        Guid ReservationId,
        Guid RoomId,
        string RoomNumber,
        int MaxOccupancy,
        IReadOnlyCollection<Guid> Occupants,
        DateTime? ActualCheckIn,
        DateTime? ActualCheckOut,
        Guid? GroupBookingId );
}
