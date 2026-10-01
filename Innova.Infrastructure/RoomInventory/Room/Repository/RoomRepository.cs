using System.Reflection;
using Dapper;
using Innova.Domain.RoomInventory.Repositories;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.RoomInventory.Room.Mapping;

namespace Innova.Infrastructure.RoomInventory.Room.Repository
{
    public sealed class RoomRepository( IDbConnectionProvider connectionProvider ):IRoomRepository
    {
        private static readonly Assembly Assembly = typeof(RoomRepository).Assembly;

        public async Task<Domain.RoomInventory.Aggregates.Room?> GetByIdAsync( RoomId id, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.Room.Sql.GetRoomByIdQuery.sql");

            RoomRow? roomRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<RoomRow>(
                                   new CommandDefinition(
                                       sql,
                                       new
                                       {
                                           Id = id.Value
                                       },
                                       connectionProvider.Transaction,
                                       cancellationToken: ct)
                               );

            return roomRow is null
                       ? null
                       : RoomMapper.ToDomain(roomRow);
        }

        public async Task<Domain.RoomInventory.Aggregates.Room?> FindVacantRoomAsync( RoomTypeId roomTypeId, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.Room.Sql.FindVacantRoom.sql");

            RoomRow? roomRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<RoomRow>(
                                   new CommandDefinition(
                                       sql,
                                       new
                                       {
                                           RoomTypeId = roomTypeId.Value
                                       },
                                       connectionProvider.Transaction,
                                       cancellationToken: ct));

            return roomRow is null
                       ? null
                       : RoomMapper.ToDomain(roomRow);
        }

        public async Task SaveAsync( Domain.RoomInventory.Aggregates.Room room, CancellationToken ct = default )
        {
            RoomRow model = RoomMapper.ToPersistenceModel(room);
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.Room.Sql.Insert.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task UpdateAsync( Domain.RoomInventory.Aggregates.Room room, CancellationToken ct = default )
        {
            RoomRow model = RoomMapper.ToPersistenceModel(room);
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.Room.Sql.Update.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task<bool> ExistsWithRoomNumberAsync( RoomNumber roomNumber, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.Room.Sql.ExistsWithRoomNumber.sql");

            return await connectionProvider.Connection.ExecuteScalarAsync<bool>(
                       new CommandDefinition(
                           sql,
                           new
                           {
                               RoomNumber = roomNumber.Value
                           },
                           connectionProvider.Transaction,
                           cancellationToken: ct));
        }
    }
}
