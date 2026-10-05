namespace Innova.Infrastructure.FrontDesk.Stay.Mapper
{
    public sealed class StaySummaryRow
    {
        public Guid Id { get; init; }
        public string AssignedRoomNumber { get; init; } = string.Empty;
        public DateTime ActualCheckIn { get; init; }
    }
}
