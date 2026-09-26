using Innova.Application.Abstractions.Messaging;
using Innova.Application.HouseKeeping.HouseKeeping.DTO;

namespace Innova.Application.HouseKeeping.HouseKeeping.Queries
{
    public sealed record ListHouseKeepingTasksQuery(
        string? Status,
        Guid? AssignedStaffId,
        Guid? RoomId ):IQuery<IReadOnlyCollection<HouseKeepingTaskSummaryDto>>;
}
