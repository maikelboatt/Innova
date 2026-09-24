using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.FrontDesk.Aggregates;
using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.FrontDesk.Reservation.Commands.AddOccupant
{
    public sealed class AddOccupantCommandHandler( IStayService stayService, IDomainEventDispatcher eventDispatcher ):ICommandHandler<AddOccupantCommand, Guid>
    {
        public async Task<Guid> HandleAsync( AddOccupantCommand command, CancellationToken ct = default )
        {
            StayId stayId = StayId.From(command.StayId);
            GuestId guestId = GuestId.From(command.GuestId);

            Stay stay = await stayService.AddOccupantAsync(stayId, guestId, ct);

            await eventDispatcher.DispatchAsync(stay.DomainEvents, ct);
            stay.ClearDomainEvents();

            return stay.Id.Value;
        }
    }
}
