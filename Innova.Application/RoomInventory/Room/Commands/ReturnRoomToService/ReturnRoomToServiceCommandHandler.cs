using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Commands.ReturnRoomToService
{
    public sealed class ReturnRoomToServiceCommandHandler( IRoomService roomService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<ReturnRoomToServiceCommand, Unit>
    {
        public async Task<Unit> HandleAsync( ReturnRoomToServiceCommand command, CancellationToken ct = default )
        {
            RoomId roomId = RoomId.From(command.RoomId.Value);

            Domain.RoomInventory.Aggregates.Room room = await roomService.ReturnToServiceAsync(roomId, ct);

            await eventDispatcher.DispatchAsync(room.DomainEvents, ct);
            room.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
