using Innova.Application.Abstractions.Services;
using Innova.Application.HouseKeeping.HouseKeeping.Exceptions;
using Innova.Domain.HouseKeeping.Aggregates;
using Innova.Domain.HouseKeeping.Repositories;
using Innova.Domain.HouseKeeping.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class HouseKeepingService( IHouseKeepingTaskRepository houseKeepingTaskRepository, IRoomService roomService ):IHouseKeepingService
    {
        public async Task<HouseKeepingTask> ScheduleAsync( Guid roomId, HouseKeepingTaskType type, CancellationToken ct = default )
        {
            HouseKeepingTask houseKeepingTask = HouseKeepingTask.Schedule(roomId, type);

            await houseKeepingTaskRepository.SaveAsync(houseKeepingTask, ct);

            return houseKeepingTask;
        }

        public async Task<HouseKeepingTask> AssignAsync( HouseKeepingTaskId taskId, StaffId staffId, CancellationToken ct = default )
        {
            HouseKeepingTask task = await GetByIdAsync(taskId, ct);

            task.AssignTo(staffId);

            await houseKeepingTaskRepository.UpdateAsync(task, ct);

            return task;
        }

        public async Task<HouseKeepingTask> CompleteAsync( HouseKeepingTaskId taskId, InspectionResult inspection, CancellationToken ct = default )
        {
            HouseKeepingTask task = await GetByIdAsync(taskId, ct);

            task.Complete(inspection);

            await houseKeepingTaskRepository.UpdateAsync(task, ct);

            if (inspection.Passed)
                await roomService.MarkVacantAsync(RoomId.From(task.RoomId), ct);

            return task;
        }

        private async Task<HouseKeepingTask> GetByIdAsync( HouseKeepingTaskId taskId, CancellationToken ct = default ) =>
            await houseKeepingTaskRepository.GetByIdAsync(taskId, ct) ?? throw new HouseKeepingTaskNotFoundException(taskId);
    }
}
