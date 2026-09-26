namespace Innova.Application.FrontDesk.Stay.DTO
{
    public sealed record StaySummaryDto( Guid StayId, string RoomNumber, DateTime ActualCheckIn );
}
