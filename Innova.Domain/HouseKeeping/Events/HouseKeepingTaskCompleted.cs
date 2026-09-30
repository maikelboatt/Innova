using Innova.Domain.Common;

namespace Innova.Domain.HouseKeeping.Events
{
    public sealed record HouseKeepingTaskCompleted(
        Guid HouseKeepingTaskId,
        Guid RoomId,
        string TaskType,
        Guid StaffId,
        DateTime CompletedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => HouseKeepingTaskId;
        string IAuditableEvent.EntityType => "HouseKeepingTask";

        string IAuditableEvent.Summary =>
            $"House keeping task {HouseKeepingTaskId} for {TaskType} in room {RoomId} has been successfully completed at {OccurredOn:g}.";

        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
