using Innova.Domain.Common;

namespace Innova.Domain.HouseKeeping.Events
{
    public sealed record HouseKeepingTaskScheduled( Guid HouseKeepingTaskId, Guid RoomId, string TaskType ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => HouseKeepingTaskId;
        string IAuditableEvent.EntityType => "HouseKeepingTask";

        string IAuditableEvent.Summary =>
            $"House keeping task {HouseKeepingTaskId} for {TaskType} has been successfully scheduled for room {RoomId} at {OccurredOn:g}.";

        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
