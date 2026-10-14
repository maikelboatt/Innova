using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.RoomInventory.RoomTypeDefinition.Commands.UpdateRoomType
{
    public sealed class UpdateRoomTypeCommandHandler( IRoomTypeService roomTypeService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<UpdateRoomTypeCommand, Unit>
    {
        public async Task<Unit> HandleAsync( UpdateRoomTypeCommand command, CancellationToken ct = default )
        {
            RoomTypeId roomTypeId = RoomTypeId.From(command.RoomTypeId);
            MaxOccupancy maxOccupancy = MaxOccupancy.Of(command.MaxOccupancy);
            Money money = Money.Of(command.BaseRateAmount);

            Domain.RoomInventory.Aggregates.RoomTypeDefinition roomType = await roomTypeService.GetByIdAsync(roomTypeId, ct);

            await roomTypeService.UpdateAsync(
                roomTypeId,
                command.RoomName,
                maxOccupancy,
                money,
                ct);

            await eventDispatcher.DispatchAsync(roomType.DomainEvents, ct);
            roomType.ClearDomainEvents();

            return Unit.Value;
        }
    }
}
