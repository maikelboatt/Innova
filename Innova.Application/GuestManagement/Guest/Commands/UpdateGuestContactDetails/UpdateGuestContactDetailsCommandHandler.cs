using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Commands.UpdateGuestContactDetails
{
    public sealed class UpdateGuestContactDetailsCommandHandler( IGuestManagementService guestManagementService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<UpdateGuestContactDetailsCommand>

    {
        public async Task<Unit> HandleAsync( UpdateGuestContactDetailsCommand command, CancellationToken ct = default )
        {
            GuestId guestId = GuestId.From(command.GuestId);
            ContactDetails contactDetails = ContactDetails.Create(command.PhoneNumber, command.Email);

            Domain.GuestManagement.Aggregates.Guest guest = await guestManagementService.UpdateContactDetailsAsync(guestId, contactDetails, ct);

            await eventDispatcher.DispatchAsync(guest.DomainEvents, ct);
            guest.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
