using Innova.Domain.Common;
using Innova.Domain.HouseKeeping.Events;
using Innova.Domain.HouseKeeping.ValueObjects;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.HouseKeeping.Aggregates
{
    public sealed class HouseKeepingTask:AggregateRoot<HouseKeepingTaskId>
    {
        private HouseKeepingTask()
        {
        }

        private HouseKeepingTask( HouseKeepingTaskId houseKeepingTaskId,
                                  Guid roomId,
                                  StaffId assignedStaffId,
                                  HouseKeepingTaskType type,
                                  HouseKeepingTaskStatus status,
                                  DateTime createdAt,
                                  DateTime? completedAt,
                                  InspectionResult inspection ):base(houseKeepingTaskId)
        {
            RoomId = roomId;
            AssignedStaffId = assignedStaffId;
            Type = type;
            Status = status;
            CreatedAt = createdAt;
            CompletedAt = completedAt;
            Inspection = inspection;
        }

        public Guid RoomId { get; }
        public StaffId? AssignedStaffId { get; private set; }
        public HouseKeepingTaskType Type { get; }
        public HouseKeepingTaskStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public InspectionResult? Inspection { get; private set; }

        public static HouseKeepingTask Schedule( Guid roomId,
                                                 HouseKeepingTaskType type )
        {
            HouseKeepingTaskId houseKeepingTaskId = HouseKeepingTaskId.New();

            HouseKeepingTask houseKeepingTask = new(
                houseKeepingTaskId,
                roomId,
                null,
                type,
                HouseKeepingTaskStatus.Pending,
                DateTime.UtcNow,
                null,
                null);

            houseKeepingTask.RaiseDomainEvent(new HouseKeepingTaskScheduled(houseKeepingTaskId.Value, roomId, type.ToString()));

            return houseKeepingTask;
        }

        public static HouseKeepingTask Reconstitute( HouseKeepingTaskId houseKeepingTaskId,
                                                     Guid roomId,
                                                     StaffId assignedStaffId,
                                                     HouseKeepingTaskType type,
                                                     HouseKeepingTaskStatus status,
                                                     DateTime createdAt,
                                                     DateTime? completedAt,
                                                     InspectionResult inspection ) => new(
            houseKeepingTaskId,
            roomId,
            assignedStaffId,
            type,
            status,
            createdAt,
            completedAt,
            inspection);

        public void AssignTo( StaffId staffId )
        {
            if (!Status.CanTransitionTo(HouseKeepingTaskStatus.InProgress))
                throw new DomainException($"Cannot assign House keeping task with status '{Status}' to Staff. Only pending statuses can be assigned");

            AssignedStaffId = staffId;
            Status = HouseKeepingTaskStatus.InProgress;

            RaiseDomainEvent(
                new HouseKeepingTaskAssigned(
                    Id.Value,
                    RoomId,
                    Type.ToString(),
                    AssignedStaffId.Value,
                    DateTime.UtcNow));
        }

        public void Complete( InspectionResult inspection )
        {
            if (!Status.CanTransitionTo(HouseKeepingTaskStatus.Completed))
                throw new DomainException("Only an in-progress task can be completed");

            Status = inspection.Passed
                         ? HouseKeepingTaskStatus.Completed
                         : HouseKeepingTaskStatus.Failed;
            Inspection = inspection;
            CompletedAt = DateTime.UtcNow;

            if (Status == HouseKeepingTaskStatus.Failed)
                RaiseDomainEvent(
                    new HouseKeepingTaskFailed(
                        Id.Value,
                        RoomId,
                        Type.ToString(),
                        AssignedStaffId.Value,
                        inspection.Notes,
                        DateTime.UtcNow));

            if (Status == HouseKeepingTaskStatus.Completed)
                RaiseDomainEvent(
                    new HouseKeepingTaskCompleted(
                        Id.Value,
                        RoomId,
                        Type.ToString(),
                        AssignedStaffId.Value,
                        DateTime.UtcNow));
        }
    }
}
