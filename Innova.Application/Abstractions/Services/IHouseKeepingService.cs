using Innova.Domain.HouseKeeping.Aggregates;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IHouseKeepingService
    {
        Task<HouseKeepingTask> ScheduleAsync( Guid roomId, HouseKeepingTaskType type, CancellationToken ct = default );

        Task<HouseKeepingTask> AssignAsync( HouseKeepingTaskId taskId, StaffId staffId, CancellationToken ct = default );

        // On a passed inspection, this also marks the Room vacant directly
        // via IRoomService — see the note in the implementation for why
        // that's now a direct call rather than routed through the
        // RoomReadyForService domain event.
        Task<HouseKeepingTask> CompleteAsync( HouseKeepingTaskId taskId, InspectionResult inspection, CancellationToken ct = default );
    }
}
