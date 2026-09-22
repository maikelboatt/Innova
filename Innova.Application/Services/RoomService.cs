using Innova.Application.Abstractions.Services;
using Innova.Application.RoomInventory.Room.Exceptions;
using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.Repositories;
using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class RoomService( IRoomRepository roomRepository ):IRoomService
    {
        public async Task<Room> CreateAsync( RoomNumber roomNumber,
                                             Floor floor,
                                             RoomTypeId roomTypeId,
                                             CancellationToken ct = default )
        {
            Room room = Room.Create(roomNumber, floor, roomTypeId);

            await roomRepository.SaveAsync(room, ct);

            return room;
        }

        public async Task<Room> OccupyAsync( RoomId roomId, CancellationToken ct = default )
        {
            Room room = await GetByIdAsync(roomId, ct);

            room.Occupy();

            await roomRepository.UpdateAsync(room, ct);

            return room;
        }

        public async Task<Room> MarkDirtyAsync( RoomId roomId, CancellationToken ct = default )
        {
            Room room = await GetByIdAsync(roomId, ct);

            room.MarkDirty();

            await roomRepository.UpdateAsync(room, ct);

            return room;
        }

        public async Task<Room> MarkVacantAsync( RoomId roomId, CancellationToken ct = default )
        {
            Room room = await GetByIdAsync(roomId, ct);

            room.MarkVacant();

            await roomRepository.UpdateAsync(room, ct);

            return room;
        }

        public async Task<Room> TakeOutOfServiceAsync( RoomId roomId, CancellationToken ct = default )
        {
            Room room = await GetByIdAsync(roomId, ct);

            room.TakeOutOfService();

            await roomRepository.UpdateAsync(room, ct);

            return room;
        }

        public async Task<Room> ReturnToServiceAsync( RoomId roomId, CancellationToken ct = default )
        {
            {
                Room room = await GetByIdAsync(roomId, ct);

                room.ReturnToService();

                await roomRepository.UpdateAsync(room, ct);

                return room;
            }
        }

        public async Task<Room?> FindVacantRoomAsync( RoomTypeId roomTypeId, CancellationToken ct = default ) =>
            await roomRepository.FindVacantRoomAsync(roomTypeId, ct);

        private async Task<Room> GetByIdAsync( RoomId roomId, CancellationToken ct = default ) =>
            await roomRepository.GetByIdAsync(roomId, ct) ?? throw new RoomNotFoundException(roomId);
    }
}
