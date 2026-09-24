using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Commands.TakeRoomOutOfService
{
    public sealed class TakeRoomOutOfServiceCommandHandler( IRoomService roomService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<TakeRoomOutOfServiceCommand, Unit>
    {
        public async Task<Unit> HandleAsync( TakeRoomOutOfServiceCommand command, CancellationToken ct = default )
        {
            RoomId roomId = RoomId.From(command.RoomId);

            Domain.RoomInventory.Aggregates.Room room = await roomService.TakeOutOfServiceAsync(roomId, ct);

            await eventDispatcher.DispatchAsync(room.DomainEvents, ct);
            room.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
