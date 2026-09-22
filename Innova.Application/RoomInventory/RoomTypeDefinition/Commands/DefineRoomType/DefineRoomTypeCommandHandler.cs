using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Commands.DefineRoomType
{
    public sealed class DefineRoomTypeCommandHandler( IRoomTypeService roomTypeService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<DefineRoomTypeCommand, Guid>
    {
        public async Task<Guid> HandleAsync( DefineRoomTypeCommand command, CancellationToken ct = default )
        {
            MaxOccupancy maxOccupancy = MaxOccupancy.Of(command.MaxOccupancy);
            Money money = Money.Of(command.BaseRateAmount, command.BaseRateCurrency);

            Domain.RoomInventory.Aggregates.RoomTypeDefinition roomType = await roomTypeService.DefineAsync(
                                                                              command.RoomName,
                                                                              maxOccupancy,
                                                                              money,
                                                                              ct);

            await eventDispatcher.DispatchAsync(roomType.DomainEvents, ct);
            roomType.ClearDomainEvents();

            return roomType.Id.Value;
        }
    }
}
