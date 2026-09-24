using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.HouseKeeping.Aggregates;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.ScheduleTask
{
    public sealed class ScheduleTaskCommandHandler( IHouseKeepingService taskService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<ScheduleTaskCommand, Guid>
    {
        public async Task<Guid> HandleAsync( ScheduleTaskCommand command, CancellationToken ct = default )
        {
            HouseKeepingTask task = await taskService.ScheduleAsync(command.RoomId, command.TaskType, ct);

            await eventDispatcher.DispatchAsync(task.DomainEvents, ct);
            task.ClearDomainEvents();

            return task.Id.Value;
        }
    }
}
