using Innova.Application.Exceptions;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.HouseKeeping.HouseKeeping.Exceptions
{
    public sealed class HouseKeepingTaskNotFoundException( HouseKeepingTaskId taskId ):ApplicationExceptions($"House keeping task {taskId} not found");
}
