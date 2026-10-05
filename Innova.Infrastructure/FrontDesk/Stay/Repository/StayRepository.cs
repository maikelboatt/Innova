using System.Reflection;
using Dapper;
using Innova.Domain.FrontDesk.Repositories;
using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Infrastructure.FrontDesk.Stay.Mapper;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.FrontDesk.Stay.Repository
{
    public sealed class StayRepository( IDbConnectionProvider connectionProvider ):IStayRepository
    {
        private static readonly Assembly Assembly = typeof(StayRepository).Assembly;

        public async Task<Domain.FrontDesk.Aggregates.Stay?> GetByIdAsync( StayId id, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.GetById.sql");
            string occupantsSql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.GetOccupantsByStayId.sql");

            StayRow? row = await connectionProvider.Connection.QuerySingleOrDefaultAsync<StayRow>(
                               new CommandDefinition(
                                   sql,
                                   new
                                   {
                                       Id = id.Value
                                   },
                                   connectionProvider.Transaction,
                                   cancellationToken: ct));

            if (row is null)
                return null;

            IEnumerable<StayOccupantRow> occupantRows = await connectionProvider.Connection.QueryAsync<StayOccupantRow>(
                                                            new CommandDefinition(
                                                                occupantsSql,
                                                                new
                                                                {
                                                                    StayId = id.Value
                                                                },
                                                                connectionProvider.Transaction,
                                                                cancellationToken: ct));

            return StayMapper.ToDomain(row, occupantRows);
        }

        public async Task SaveAsync( Domain.FrontDesk.Aggregates.Stay stay, CancellationToken ct = default )
        {
            StayPersistenceModel model = StayMapper.ToPersistenceModel(stay);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.Insert.sql");
            string occupantSql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.InsertOccupant.sql");

            CommandDefinition cmd = new(
                sql,
                model.Stay,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);

            if (model.Occupants.Count > 0)
            {
                CommandDefinition occupantCmd = new(
                    occupantSql,
                    model.Occupants,
                    connectionProvider.Transaction,
                    cancellationToken: ct);

                await connectionProvider.Connection.ExecuteAsync(occupantCmd);
            }
        }

        public async Task UpdateAsync( Domain.FrontDesk.Aggregates.Stay stay, CancellationToken ct = default )
        {
            StayPersistenceModel model = StayMapper.ToPersistenceModel(stay);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.Update.sql");
            string insertOccupantSql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.InsertOccupant.sql");
            string deleteOccupantSql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.DeleteOccupantsByStayId.sql");

            await connectionProvider.Connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    model.Stay,
                    connectionProvider.Transaction,
                    cancellationToken: ct));

            // Delete-then-reinsert: simplest correct strategy for value-object collections
            await connectionProvider.Connection.ExecuteAsync(
                new CommandDefinition(
                    deleteOccupantSql,
                    new
                    {
                        StayId = model.Stay.Id
                    },
                    connectionProvider.Transaction,
                    cancellationToken: ct));

            if (model.Occupants.Count > 0)
            {
                await connectionProvider.Connection.ExecuteAsync(
                    new CommandDefinition(
                        insertOccupantSql,
                        model.Occupants,
                        connectionProvider.Transaction,
                        cancellationToken: ct));
            }
        }
    }
}
