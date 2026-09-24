using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.Reservations.GroupBooking.Commands.OpenGroupBooking
{
    public sealed class OpenGroupBookingCommandHandler( IGroupBookingService groupBookingService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<OpenGroupBookingCommand, Guid>
    {
        public async Task<Guid> HandleAsync( OpenGroupBookingCommand command, CancellationToken ct = default )
        {
            GuestId guestId = GuestId.From(command.OrganizerGuestId);

            Domain.Reservations.Aggregates.GroupBooking booking = await groupBookingService.OpenAsync(guestId, command.GroupName, ct);

            await eventDispatcher.DispatchAsync(booking.DomainEvents, ct);

            booking.ClearDomainEvents();

            return booking.Id.Value;
        }
    }
}
