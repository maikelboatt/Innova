using Innova.Application.Exceptions;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.HouseKeeping.HouseKeeping.Exceptions
{
    public sealed class DuplicateHouseKeepingTaskException( Guid roomId, HouseKeepingTaskId taskId )
        :ApplicationExceptions($"Room '{roomId}' already has a house keeping task '{taskId}'");
}
