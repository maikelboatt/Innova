using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.HouseKeeping.HouseKeeping.Exceptions
{
    public sealed class DuplicateHouseKeepingTaskException( Guid roomId, HouseKeepingTaskId taskId )
        :ApplicationException($"Room '{roomId}' already has a house keeping task '{taskId}'");
}
