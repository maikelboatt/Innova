namespace Innova.Application.HouseKeeping.HouseKeeping.DTO
{
    public sealed record HouseKeepingTaskDto(
        Guid TaskId,
        Guid RoomId,
        string Type,
        string Status,
        Guid? AssignedStaffId,
        DateTime CreatedAt,
        DateTime? CompletedAt,
        bool? InspectionPassed,
        string? InspectionNotes );
}
