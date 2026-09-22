using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Application.GuestManagement.Guest.Exceptions;
using Innova.Domain.GuestManagement.Repositories;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Commands.DeactivateGuest
{
    public sealed class DeactivateGuestCommandHandler(
        IGuestManagementService guestManagementService,
        IDomainEventDispatcher eventDispatcher,
        IGuestRepository guestRepository )
        :ICommandHandler<DeactivateGuestCommand, Unit>
    {
        public async Task<Unit> HandleAsync( DeactivateGuestCommand command, CancellationToken ct = default )
        {
            GuestId guestId = GuestId.From(command.GuestId);
            await guestManagementService.DeactivateAsync(guestId, ct);

            Domain.GuestManagement.Aggregates.Guest guest = await guestRepository.GetByIdAsync(guestId, ct) ?? throw new GuestNotFoundException(guestId);

            await eventDispatcher.DispatchAsync(guest.DomainEvents, ct);
            guest.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
