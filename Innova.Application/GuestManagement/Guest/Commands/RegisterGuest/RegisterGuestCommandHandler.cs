using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Commands.RegisterGuest
{
    public sealed class RegisterGuestCommandHandler( IGuestManagementService guestManagementService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<RegisterGuestCommand, Guid>
    {
        public async Task<Guid> HandleAsync( RegisterGuestCommand command, CancellationToken ct = default )
        {
            // Convert raw primitives into Domain Value Objects
            PersonName personName = PersonName.Create(command.FirstName, command.LastName, command?.MiddleName);
            DateOfBirth dateOfBirth = DateOfBirth.Create(command.DateOfBirth);
            ContactDetails contactDetails = ContactDetails.Create(command.PhoneNumber, command.Email);
            IdentityDocument identityDocument = IdentityDocument.Create(command.IdentityDocumentType, command.IdentityDocumentNumber);

            // Call Application Services
            Domain.GuestManagement.Aggregates.Guest guest = await guestManagementService.RegisterAsync(
                                                                personName,
                                                                dateOfBirth,
                                                                contactDetails,
                                                                identityDocument,
                                                                ct);

            // Dispatch and clear events
            await eventDispatcher.DispatchAsync(guest.DomainEvents, ct);
            guest.ClearDomainEvents();

            return guest.Id.Value;
        }
    }
}
