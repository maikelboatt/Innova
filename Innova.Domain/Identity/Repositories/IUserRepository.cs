using Innova.Domain.Identity.Aggregates;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Identity.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync( UserId userId, CancellationToken ct = default );

        Task<User?> GetByUsernameAsync( string username, CancellationToken ct = default );

        Task SaveAsync( User user, CancellationToken ct = default );

        Task UpdateAsync( User user, CancellationToken ct = default );
    }
}
