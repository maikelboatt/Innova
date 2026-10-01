using Innova.Application.HouseKeeping.HouseKeeping.DTO;
using Innova.Infrastructure.HouseKeeping.HouseKeeping.Mapping;

namespace Innova.Infrastructure.HouseKeeping.HouseKeeping.Queries
{
    public static class HouseKeepingTaskQueryHandlerMapper
    {
        public static IReadOnlyCollection<HouseKeepingTaskDto> MapToDto( IEnumerable<HouseKeepingTaskRow> rows ) => rows
            .Select(ToDto)
            .ToList()
            .AsReadOnly();

        public static HouseKeepingTaskDto ToDto( HouseKeepingTaskRow row ) => new(
            row.Id,
            row.RoomId,
            row.Type,
            row.Status,
            row.AssignedStaffId,
            row.CreatedAt,
            row.CompletedAt,
            row.InspectionPassed,
            row.InspectionNotes);

        public static IReadOnlyCollection<HouseKeepingTaskSummaryDto> MapToSummaryDto( IEnumerable<HouseKeepingTaskSummaryRow> rows ) => rows
            .Select(ToSummaryDto)
            .ToList()
            .AsReadOnly();

        public static HouseKeepingTaskSummaryDto ToSummaryDto( HouseKeepingTaskSummaryRow row ) => new(
            row.Id,
            row.RoomId,
            row.Type,
            row.Status,
            row.AssignedStaffId);
    }
}
