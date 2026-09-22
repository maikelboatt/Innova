using Innova.Domain.GuestManagement.Aggregates;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Domain.GuestManagement.Repositories
{
    public interface IGuestRepository
    {
        Task<Guest?> GetByIdAsync( GuestId guestId, CancellationToken ct = default );

        Task SaveAsync( Guest guest, CancellationToken ct = default );

        Task UpdateAsync( Guest guest, CancellationToken ct = default );

        Task<bool> ExistsWithIdentityDocumentAsync( IdentityDocument identityDocument, CancellationToken ct = default );
    }
}
