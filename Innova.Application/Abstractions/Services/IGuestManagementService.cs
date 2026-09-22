using Innova.Domain.GuestManagement.Aggregates;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IGuestManagementService
    {
        Task<Guest> RegisterAsync( PersonName personName,
                                   DateOfBirth dateOfBirth,
                                   ContactDetails contactDetails,
                                   IdentityDocument identityDocument,
                                   CancellationToken ct = default );

        Task<Guest> UpdateContactDetailsAsync( GuestId guestId, ContactDetails contactDetails, CancellationToken ct = default );

        Task<Guest> ChangeIdentityDocumentAsync( GuestId guestId, IdentityDocument identityDocument, CancellationToken ct = default );

        Task DeactivateAsync( GuestId guestId, CancellationToken ct = default );

        Task ReactivateAsync( GuestId guestId, CancellationToken ct = default );
    }
}
