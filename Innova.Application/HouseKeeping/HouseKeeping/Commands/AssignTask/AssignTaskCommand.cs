using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.AssignTask
{
    public sealed record AssignTaskCommand( Guid HouseKeepingTaskId, Guid StaffId ):ICommand<Guid>;
}
