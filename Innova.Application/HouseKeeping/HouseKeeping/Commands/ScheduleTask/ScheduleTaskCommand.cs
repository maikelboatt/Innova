using Innova.Application.Abstractions.Messaging;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.ScheduleTask
{
    public sealed record ScheduleTaskCommand( Guid RoomId, HouseKeepingTaskType TaskType ):ICommand<Guid>;
}
