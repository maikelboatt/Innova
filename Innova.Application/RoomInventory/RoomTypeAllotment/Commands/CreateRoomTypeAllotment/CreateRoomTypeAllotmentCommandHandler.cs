using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment
{
    public sealed class CreateRoomTypeAllotmentCommandHandler( IRoomAvailabilityService availabilityService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<CreateRoomTypeAllotmentCommand, Guid>
    {
        public async Task<Guid> HandleAsync( CreateRoomTypeAllotmentCommand command, CancellationToken ct = default )
        {
            RoomTypeId roomTypeId = RoomTypeId.From(command.RoomTypeId.Value);
            DateRange stayPeriod = DateRange.Of(command.StayPeriod.Start, command.StayPeriod.End);

            Domain.RoomInventory.Aggregates.RoomTypeAllotment roomTypeAllotment =
                await availabilityService.CreateAsync(
                    roomTypeId,
                    stayPeriod,
                    command.TotalRooms,
                    ct);

            await eventDispatcher.DispatchAsync(roomTypeAllotment.DomainEvents, ct);
            roomTypeAllotment.ClearDomainEvents();

            return roomTypeAllotment.Id.Value;
        }
    }
}
