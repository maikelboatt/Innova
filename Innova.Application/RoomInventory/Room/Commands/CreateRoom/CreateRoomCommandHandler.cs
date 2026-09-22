using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.RoomInventory.Room.Commands.CreateRoom
{
    public sealed class CreateRoomCommandHandler( IRoomService roomService, IDomainEventDispatcher eventDispatcher ):ICommandHandler<CreateRoomCommand, Guid>
    {
        public async Task<Guid> HandleAsync( CreateRoomCommand command, CancellationToken ct = default )
        {
            RoomNumber roomNumber = RoomNumber.Of(command.RoomNumber);
            Floor floor = Floor.Of(command.FloorLevel, command.Wing);
            RoomTypeId roomTypeId = RoomTypeId.From(command.RoomTypeId);

            Domain.RoomInventory.Aggregates.Room room = await roomService.CreateAsync(
                                                            roomNumber,
                                                            floor,
                                                            roomTypeId,
                                                            ct);

            await eventDispatcher.DispatchAsync(room.DomainEvents, ct);
            room.ClearDomainEvents();

            return room.Id.Value;
        }
    }
}
