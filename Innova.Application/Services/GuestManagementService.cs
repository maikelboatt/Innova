using Innova.Application.Abstractions.Services;
using Innova.Application.GuestManagement.Guest.Exceptions;
using Innova.Domain.GuestManagement.Aggregates;
using Innova.Domain.GuestManagement.Repositories;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class GuestManagementService( IGuestRepository guestRepository ):IGuestManagementService
    {
        public async Task<Guest> RegisterAsync( PersonName personName,
                                                DateOfBirth dateOfBirth,
                                                ContactDetails contactDetails,
                                                IdentityDocument identityDocument,
                                                CancellationToken ct = default )
        {
            bool alreadyExists = await guestRepository.ExistsWithIdentityDocumentAsync(identityDocument, ct);

            if (alreadyExists)
                throw new DuplicateGuestIdentityDocumentException(identityDocument);

            Guest guest = Guest.Create(
                personName,
                dateOfBirth,
                contactDetails,
                identityDocument);

            await guestRepository.SaveAsync(guest, ct);

            return guest;
        }

        public async Task<Guest> UpdateContactDetailsAsync( GuestId guestId, ContactDetails contactDetails, CancellationToken ct = default )
        {
            Guest guest = await GetByIdAsync(guestId, ct);

            guest.UpdateContactDetails(contactDetails);

            await guestRepository.UpdateAsync(guest, ct);

            return guest;
        }

        public async Task<Guest> ChangeIdentityDocumentAsync( GuestId guestId, IdentityDocument identityDocument, CancellationToken ct = default )
        {
            Guest guest = await GetByIdAsync(guestId, ct);

            bool alreadyExists = await guestRepository.ExistsWithIdentityDocumentAsync(identityDocument, ct);

            if (alreadyExists)
                throw new DuplicateGuestIdentityDocumentException(identityDocument);

            guest.ChangeIdentityDocument(identityDocument);

            await guestRepository.UpdateAsync(guest, ct);

            return guest;
        }

        public async Task DeactivateAsync( GuestId guestId, CancellationToken ct = default )
        {
            Guest guest = await GetByIdAsync(guestId, ct);

            guest.Deactivate();

            await guestRepository.UpdateAsync(guest, ct);
        }

        public async Task ReactivateAsync( GuestId guestId, CancellationToken ct = default )
        {
            Guest guest = await GetByIdAsync(guestId, ct);

            guest.Reactivate();

            await guestRepository.UpdateAsync(guest, ct);
        }

        private async Task<Guest> GetByIdAsync( GuestId guestId, CancellationToken ct = default ) =>
            await guestRepository.GetByIdAsync(guestId, ct) ?? throw new GuestNotFoundException(guestId);
    }
}
