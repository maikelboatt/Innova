using Innova.Domain.FrontDesk.Aggregates;
using Innova.Domain.FrontDesk.ValueObjects;

namespace Innova.Domain.FrontDesk.Repositories
{
    public interface IStayRepository
    {
        Task<Stay?> GetByIdAsync( StayId id, CancellationToken ct = default );

        Task SaveAsync( Stay stay, CancellationToken ct = default );

        Task UpdateAsync( Stay stay, CancellationToken ct = default );
    }
}
