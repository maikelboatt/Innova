using Innova.Application.Abstractions.Services;
using Innova.Application.RoomInventory.RoomTypeAllotment.Exceptions;
using Innova.Application.RoomInventory.RoomTypeDefinition.Exceptions;
using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.Repositories;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class RoomAvailabilityService( IRoomTypeAllotmentRepository allotmentRepository, IRoomTypeDefinitionRepository typeDefinitionRepository )
        :IRoomAvailabilityService
    {
        public async Task<RoomTypeAllotment> CreateAsync( RoomTypeId roomTypeId,
                                                          DateRange period,
                                                          int totalRooms,
                                                          CancellationToken ct = default )
        {
            _ = await typeDefinitionRepository.GetByIdAsync(roomTypeId, ct) ?? throw new RoomTypeDefinitionNotFoundException(roomTypeId);

            RoomTypeAllotment allotment = RoomTypeAllotment.Create(roomTypeId, period, totalRooms);

            await allotmentRepository.SaveAsync(allotment, ct);

            return allotment;
        }

        public async Task<RoomTypeAllotment> ReserveCapacityAsync( RoomTypeId roomTypeId,
                                                                   DateRange period,
                                                                   int rooms = 1,
                                                                   CancellationToken ct = default )
        {
            RoomTypeAllotment allotment = await GetAllotmentAsync(
                                              roomTypeId,
                                              period,
                                              rooms,
                                              ct);

            allotment.Reserve(rooms);

            await allotmentRepository.UpdateAsync(allotment, ct);

            return allotment;
        }

        public async Task<RoomTypeAllotment> ReleaseCapacityAsync( RoomTypeId roomTypeId,
                                                                   DateRange period,
                                                                   int rooms,
                                                                   CancellationToken ct = default )
        {
            RoomTypeAllotment allotment = await GetAllotmentAsync(
                                              roomTypeId,
                                              period,
                                              rooms,
                                              ct);

            allotment.Release(rooms);

            await allotmentRepository.UpdateAsync(allotment, ct);

            return allotment;
        }

        private async Task<RoomTypeAllotment> GetAllotmentAsync( RoomTypeId roomTypeId,
                                                                 DateRange period,
                                                                 int rooms,
                                                                 CancellationToken ct = default ) => await allotmentRepository.GetByRoomTypeAndPeriodAsync(
                                                                                                         roomTypeId,
                                                                                                         period.Start,
                                                                                                         period.End,
                                                                                                         ct) ?? throw new NoAllotmentForTypeAndPeriodException(
                                                                                                         roomTypeId,
                                                                                                         period);
    }
}
