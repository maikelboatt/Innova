namespace Innova.Infrastructure.HouseKeeping.HouseKeeping.Mapping
{
    public sealed class HouseKeepingTaskSummaryRow
    {
        public Guid Id { get; init; }
        public Guid RoomId { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid? AssignedStaffId { get; init; }
    }
}
