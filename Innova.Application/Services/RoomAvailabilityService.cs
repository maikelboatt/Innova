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
        public async Task<IReadOnlyCollection<RoomTypeAllotment>> CreateAsync( RoomTypeId roomTypeId,
                                                                               DateRange period,
                                                                               int totalRooms,
                                                                               CancellationToken ct = default )
        {
            _ = await typeDefinitionRepository.GetByIdAsync(roomTypeId, ct) ?? throw new RoomTypeDefinitionNotFoundException(roomTypeId);

            List<RoomTypeAllotment> allotments = [];

            for (DateOnly date = period.Start; date < period.End; date = date.AddDays(1))
            {
                RoomTypeAllotment allotment = RoomTypeAllotment.Create(roomTypeId, date, totalRooms);
                await allotmentRepository.SaveAsync(allotment, ct);
                allotments.Add(allotment);
            }

            return allotments;
        }

        public async Task<IReadOnlyCollection<RoomTypeAllotment>> ReserveCapacityAsync( RoomTypeId roomTypeId,
                                                                                        DateRange stayPeriod,
                                                                                        int rooms,
                                                                                        CancellationToken ct = default )
        {
            List<RoomTypeAllotment> reserved = [];

            for (DateOnly date = stayPeriod.Start; date < stayPeriod.End; date = date.AddDays(1))
            {
                RoomTypeAllotment allotment = await GetAllotmentAsync(
                                                  roomTypeId,
                                                  stayPeriod,
                                                  rooms,
                                                  ct);
                allotment.Reserve(rooms);
                await allotmentRepository.UpdateAsync(allotment, ct);
                reserved.Add(allotment);
            }

            return reserved;
        }

        public async Task<IReadOnlyCollection<RoomTypeAllotment>> ReleaseCapacityAsync( RoomTypeId roomTypeId,
                                                                                        DateRange stayPeriod,
                                                                                        int rooms,
                                                                                        CancellationToken ct = default )
        {
            List<RoomTypeAllotment> released = [];

            for (DateOnly date = stayPeriod.Start; date < stayPeriod.End; date = date.AddDays(1))
            {
                RoomTypeAllotment allotment = await GetAllotmentAsync(
                                                  roomTypeId,
                                                  stayPeriod,
                                                  rooms,
                                                  ct);

                allotment.Release(rooms);
                await allotmentRepository.UpdateAsync(allotment, ct);
                released.Add(allotment);
            }

            return released;
        }

        private async Task<RoomTypeAllotment> GetAllotmentAsync( RoomTypeId roomTypeId,
                                                                 DateRange period,
                                                                 int rooms,
                                                                 CancellationToken ct = default ) => await allotmentRepository.GetByRoomTypeAndDateAsync(
                                                                                                         roomTypeId,
                                                                                                         period.Start,
                                                                                                         ct) ?? throw new NoAllotmentForTypeAndPeriodException(
                                                                                                         roomTypeId,
                                                                                                         period);
    }
}
