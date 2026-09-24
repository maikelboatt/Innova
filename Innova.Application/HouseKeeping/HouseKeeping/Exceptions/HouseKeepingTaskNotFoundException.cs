using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.HouseKeeping.HouseKeeping.Exceptions
{
    public sealed class HouseKeepingTaskNotFoundException( HouseKeepingTaskId taskId ):ApplicationException($"House keeping task {taskId} not found");
}
