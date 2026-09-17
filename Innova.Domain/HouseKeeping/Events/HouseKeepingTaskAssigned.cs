using Innova.Domain.Common;

namespace Innova.Domain.HouseKeeping.Events
{
    public sealed record HouseKeepingTaskAssigned(
        Guid HouseKeepingTaskId,
        Guid RoomId,
        string TaskType,
        Guid StaffId,
        DateTime AssignedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
