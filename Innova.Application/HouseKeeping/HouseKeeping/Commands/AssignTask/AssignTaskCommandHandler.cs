using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.HouseKeeping.Aggregates;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.AssignTask
{
    public sealed class AssignTaskCommandHandler( IHouseKeepingService taskService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<AssignTaskCommand, Guid>
    {
        public async Task<Guid> HandleAsync( AssignTaskCommand command, CancellationToken ct = default )
        {
            HouseKeepingTaskId taskId = HouseKeepingTaskId.From(command.HouseKeepingTaskId);
            StaffId staffId = StaffId.From(command.HouseKeepingTaskId);

            HouseKeepingTask task = await taskService.AssignAsync(taskId, staffId, ct);

            await eventDispatcher.DispatchAsync(task.DomainEvents, ct);
            task.ClearDomainEvents();

            return task.Id.Value;
        }
    }
}
