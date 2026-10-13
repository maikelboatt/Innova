using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping.Forms
{
    public sealed record ScheduleHouseKeepingParameter( Guid RoomId, HouseKeepingTaskType Type );
}
