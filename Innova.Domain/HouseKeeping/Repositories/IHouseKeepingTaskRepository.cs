using Innova.Domain.HouseKeeping.Aggregates;
using Innova.Domain.HouseKeeping.ValueObjects;

namespace Innova.Domain.HouseKeeping.Repositories
{
    public interface IHouseKeepingTaskRepository
    {
        Task<HouseKeepingTask?> GetByIdAsync( HouseKeepingTaskId id, CancellationToken ct = default );

        Task SaveAsync( HouseKeepingTask task, CancellationToken ct = default );

        Task UpdateAsync( HouseKeepingTask task, CancellationToken ct = default );
    }
}
