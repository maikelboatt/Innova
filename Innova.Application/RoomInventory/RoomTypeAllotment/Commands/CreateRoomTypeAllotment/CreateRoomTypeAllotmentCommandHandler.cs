using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment
{
    public sealed class CreateRoomTypeAllotmentCommandHandler( IRoomAvailabilityService availabilityService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<CreateRoomTypeAllotmentCommand, IReadOnlyCollection<Guid>>
    {
        public async Task<IReadOnlyCollection<Guid>> HandleAsync( CreateRoomTypeAllotmentCommand command, CancellationToken ct = default )
        {
            RoomTypeId roomTypeId = RoomTypeId.From(command.RoomTypeId);
            DateRange stayPeriod = DateRange.Of(command.CheckIn, command.CheckOut);

            IReadOnlyCollection<Domain.RoomInventory.Aggregates.RoomTypeAllotment> roomTypeAllotments =
                await availabilityService.CreateAsync(
                    roomTypeId,
                    stayPeriod,
                    command.TotalRooms,
                    ct);

            List<Guid> allotmentGuids = [];

            foreach (Domain.RoomInventory.Aggregates.RoomTypeAllotment allotment in roomTypeAllotments)
            {
                await eventDispatcher.DispatchAsync(allotment.DomainEvents, ct);
                allotment.ClearDomainEvents();
                allotmentGuids.Add(allotment.Id.Value);
            }

            return allotmentGuids.AsReadOnly();
        }
    }
}
