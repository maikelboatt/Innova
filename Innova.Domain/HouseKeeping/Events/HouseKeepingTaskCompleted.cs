using Innova.Domain.Common;

namespace Innova.Domain.HouseKeeping.Events
{
    public sealed record HouseKeepingTaskCompleted(
        Guid HouseKeepingTaskId,
        Guid RoomId,
        string TaskType,
        Guid StaffId,
        DateTime CompletedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
