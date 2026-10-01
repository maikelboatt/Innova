using Innova.Domain.HouseKeeping.Aggregates;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Infrastructure.HouseKeeping.HouseKeeping.Mapping
{
    public static class HouseKeepingTaskMapper
    {
        public static HouseKeepingTaskRow ToPersistenceModel( HouseKeepingTask task ) => new()
                                                                                         {
                                                                                             Id = task.Id.Value,
                                                                                             RoomId = task.RoomId,
                                                                                             Type = task.Type.ToString(),
                                                                                             Status = task.Status.ToString(),
                                                                                             AssignedStaffId = task.AssignedStaffId?.Value,
                                                                                             CreatedAt = task.CreatedAt,
                                                                                             CompletedAt = task.CompletedAt,
                                                                                             InspectionPassed = task.Inspection?.Passed,
                                                                                             InspectionNotes = task.Inspection?.Notes
                                                                                         };

        public static IReadOnlyCollection<HouseKeepingTask> ToDomain( IEnumerable<HouseKeepingTaskRow> rows ) => rows
            .Select(ToDomain)
            .ToList()
            .AsReadOnly();

        public static HouseKeepingTask ToDomain( HouseKeepingTaskRow row )
        {
            InspectionResult? inspection = null;

            if (row.InspectionPassed.HasValue)
            {
                inspection = InspectionResult.Of(row.InspectionPassed.Value, row.InspectionNotes);
            }

            return HouseKeepingTask.Reconstitute(
                HouseKeepingTaskId.From(row.Id),
                row.RoomId,
                row.AssignedStaffId.HasValue
                    ? StaffId.From(row.AssignedStaffId.Value)
                    : null,
                Enum.Parse<HouseKeepingTaskType>(row.Type),
                MapStringToStatus(row.Status),
                row.CreatedAt,
                row.CompletedAt,
                inspection);
        }

        private static HouseKeepingTaskStatus MapStringToStatus( string status )
        {
            return status switch
                   {
                       "Pending"    => HouseKeepingTaskStatus.Pending,
                       "InProgress" => HouseKeepingTaskStatus.InProgress,
                       "Completed"  => HouseKeepingTaskStatus.Completed,
                       "Failed"     => HouseKeepingTaskStatus.Failed,
                       _            => throw new ArgumentException($"Invalid house keeping status {status}")
                   };
        }
    }
}
