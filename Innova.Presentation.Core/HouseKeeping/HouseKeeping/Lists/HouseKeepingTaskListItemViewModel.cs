using Innova.Application.HouseKeeping.HouseKeeping.DTO;

namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping.Lists
{
    public sealed class HouseKeepingTaskListItemViewModel
    {
        public HouseKeepingTaskListItemViewModel( HouseKeepingTaskSummaryDto dto )
        {
            TaskId = dto.TaskId;
            RoomId = dto.RoomId;
            Type = dto.Type;
            Status = dto.Status;
            AssignedStaffId = dto.AssignedStaffId;

            TypeLabel = dto.Type;
            StatusLabel = GetStatusLabel(dto.Status);
            StatusBadge = GetStatusBadge(dto.Status);

            AssignmentLabel = dto.AssignedStaffId.HasValue
                                  ? "Assigned"
                                  : "Unassigned";
        }

        public Guid TaskId { get; }
        public Guid RoomId { get; }
        public string Type { get; }
        public string TypeLabel { get; }
        public string Status { get; }
        public string StatusLabel { get; }
        public string StatusBadge { get; }
        public Guid? AssignedStaffId { get; }
        public string AssignmentLabel { get; }

        private static string GetStatusLabel( string status ) => status switch
                                                                 {
                                                                     "Pending"    => "Pending",
                                                                     "InProgress" => "In Progress",
                                                                     "Completed"  => "Completed",
                                                                     _            => status
                                                                 };

        private static string GetStatusBadge( string status ) => status switch
                                                                 {
                                                                     "Pending"    => "Warning",
                                                                     "InProgress" => "Info",
                                                                     "Completed"  => "Success",
                                                                     _            => "Default"
                                                                 };
    }
}
