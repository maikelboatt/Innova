namespace Innova.Infrastructure.HouseKeeping.HouseKeeping.Mapping
{
    public sealed class HouseKeepingTaskRow
    {
        public Guid Id { get; init; }
        public Guid RoomId { get; init; }
        public string Type { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public Guid? AssignedStaffId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? CompletedAt { get; init; }
        public bool? InspectionPassed { get; init; }
        public string? InspectionNotes { get; init; }
    }
}
