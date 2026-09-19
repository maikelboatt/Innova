using Innova.Domain.GuestManagement.Aggregates;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Domain.GuestManagement.Repositories
{
    public interface IGuestRepository
    {
        Task<Guest?> GetByIdAsync( GuestId guestId, CancellationToken ct = default );

        Task SaveAsync( Guest guest, CancellationToken ct = default );

        Task UpdateAsync( Guest guest, CancellationToken ct = default );

        Task<bool> ExistByIdentityAsync( PersonName personName,
                                         DateOfBirth dateOfBirth,
                                         ContactDetails contactDetails,
                                         IdentityDocument identityDocument,
                                         CancellationToken ct = default );
    }
}
