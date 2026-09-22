using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.GuestManagement.Repositories;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Commands.ChangeGuestIdentityDocument
{
    public sealed class ChangeGuestIdentityDocumentCommandHandler(
        IGuestManagementService guestManagementService,
        IDomainEventDispatcher eventDispatcher,
        IGuestRepository guestRepository )
        :ICommandHandler<ChangeGuestIdentityDocumentCommand>
    {
        public async Task<Unit> HandleAsync( ChangeGuestIdentityDocumentCommand command, CancellationToken ct = default )
        {
            GuestId guestId = GuestId.From(command.GuestId);
            IdentityDocument identityDocument = IdentityDocument.Create(command.IdentityDocumentType, command.IdentityDocumentNumber);

            Domain.GuestManagement.Aggregates.Guest guest = await guestManagementService.ChangeIdentityDocumentAsync(guestId, identityDocument, ct);

            await eventDispatcher.DispatchAsync(guest.DomainEvents, ct);
            guest.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
