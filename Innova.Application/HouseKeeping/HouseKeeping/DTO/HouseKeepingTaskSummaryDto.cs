namespace Innova.Application.HouseKeeping.HouseKeeping.DTO
{
    public sealed record HouseKeepingTaskSummaryDto(
        Guid TaskId,
        Guid RoomId,
        string Type,
        string Status,
        Guid? AssignedStaffId );
}
