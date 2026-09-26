using Innova.Application.Abstractions.Messaging;
using Innova.Application.HouseKeeping.HouseKeeping.DTO;

namespace Innova.Application.HouseKeeping.HouseKeeping.Queries
{
    public sealed record GetHouseKeepingTaskByIdQuery( Guid TaskId ):IQuery<HouseKeepingTaskDto?>;
}
