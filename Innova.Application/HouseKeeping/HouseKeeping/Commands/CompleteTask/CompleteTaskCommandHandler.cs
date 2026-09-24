using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.HouseKeeping.Aggregates;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.HouseKeeping.HouseKeeping.Commands.CompleteTask
{
    public sealed class CompleteTaskCommandHandler( IHouseKeepingService taskService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<CompleteTaskCommand, Guid>
    {
        public async Task<Guid> HandleAsync( CompleteTaskCommand command, CancellationToken ct = default )
        {
            HouseKeepingTaskId taskId = HouseKeepingTaskId.From(command.HouseKeepingId);
            InspectionResult result = InspectionResult.Of(command.InspectionPassed, command.InspectionNotes);

            HouseKeepingTask task = await taskService.CompleteAsync(taskId, result, ct);

            await eventDispatcher.DispatchAsync(task.DomainEvents, ct);
            task.ClearDomainEvents();

            return task.Id.Value;
        }
    }
}
