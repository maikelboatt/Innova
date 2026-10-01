using System.Reflection;
using Dapper;
using Innova.Domain.HouseKeeping.Aggregates;
using Innova.Domain.HouseKeeping.Repositories;
using Innova.Domain.HouseKeeping.ValueObjects;
using Innova.Infrastructure.HouseKeeping.HouseKeeping.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.HouseKeeping.HouseKeeping.Repository
{
    public sealed class HouseKeepingTaskRepository( IDbConnectionProvider connectionProvider ):IHouseKeepingTaskRepository
    {
        private static readonly Assembly Assembly = typeof(HouseKeepingTaskRepository).Assembly;

        public async Task<HouseKeepingTask?> GetByIdAsync( HouseKeepingTaskId id, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.HouseKeeping.HouseKeeping.Sql.GetById.sql");

            HouseKeepingTaskRow? houseKeepingRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<HouseKeepingTaskRow>(
                                                       new CommandDefinition(
                                                           sql,
                                                           new
                                                           {
                                                               Id = id.Value
                                                           },
                                                           connectionProvider.Transaction,
                                                           cancellationToken: ct));

            return houseKeepingRow is null
                       ? null
                       : HouseKeepingTaskMapper.ToDomain(houseKeepingRow);
        }

        public async Task<HouseKeepingTask?> GetByRoomIdAsync( Guid roomId, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.HouseKeeping.HouseKeeping.Sql.GetByRoomId.sql");

            HouseKeepingTaskRow? houseKeepingRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<HouseKeepingTaskRow>(
                                                       new CommandDefinition(
                                                           sql,
                                                           new
                                                           {
                                                               RoomId = roomId
                                                           },
                                                           connectionProvider.Transaction,
                                                           cancellationToken: ct));

            return houseKeepingRow is null
                       ? null
                       : HouseKeepingTaskMapper.ToDomain(houseKeepingRow);
        }

        public async Task SaveAsync( HouseKeepingTask task, CancellationToken ct = default )
        {
            HouseKeepingTaskRow model = HouseKeepingTaskMapper.ToPersistenceModel(task);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.HouseKeeping.HouseKeeping.Sql.Insert.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task UpdateAsync( HouseKeepingTask task, CancellationToken ct = default )
        {
            HouseKeepingTaskRow model = HouseKeepingTaskMapper.ToPersistenceModel(task);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.HouseKeeping.HouseKeeping.Sql.Update.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }
    }
}
