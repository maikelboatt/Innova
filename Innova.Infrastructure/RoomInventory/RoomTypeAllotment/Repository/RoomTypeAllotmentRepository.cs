using System.Reflection;
using Dapper;
using Innova.Domain.RoomInventory.Repositories;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Mapping;

namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Repository
{
    public sealed class RoomTypeAllotmentRepository( IDbConnectionProvider connectionProvider ):IRoomTypeAllotmentRepository
    {
        private static readonly Assembly Assembly = typeof(RoomTypeAllotmentRepository).Assembly;

        public async Task<Domain.RoomInventory.Aggregates.RoomTypeAllotment?> GetByIdAsync( RoomTypeAllotmentId id, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Sql.GetById.sql");

            AllotmentRow? allotmentRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<AllotmentRow>(
                                             new CommandDefinition(
                                                 sql,
                                                 new
                                                 {
                                                     Id = id.Value
                                                 },
                                                 connectionProvider.Transaction,
                                                 cancellationToken: ct));


            return allotmentRow is null
                       ? null
                       : RoomTypeAllotmentMapper.ToDomain(allotmentRow);
        }

        public async Task<Domain.RoomInventory.Aggregates.RoomTypeAllotment?> GetByRoomTypeAndDateAsync( RoomTypeId roomTypeId,
                                                                                                         DateOnly date,
                                                                                                         CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Sql.GetByRoomTypeAndDate.sql");

            AllotmentRow? allotmentRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<AllotmentRow>(
                                             new CommandDefinition(
                                                 sql,
                                                 new
                                                 {
                                                     RoomTypeId = roomTypeId.Value,
                                                     Date = date
                                                 },
                                                 connectionProvider.Transaction,
                                                 cancellationToken: ct));


            return allotmentRow is null
                       ? null
                       : RoomTypeAllotmentMapper.ToDomain(allotmentRow);
        }

        public async Task SaveAsync( Domain.RoomInventory.Aggregates.RoomTypeAllotment allotment, CancellationToken ct = default )
        {
            AllotmentRow model = RoomTypeAllotmentMapper.ToPersistenceModel(allotment);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Sql.Insert.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task UpdateAsync( Domain.RoomInventory.Aggregates.RoomTypeAllotment allotment, CancellationToken ct = default )
        {
            AllotmentRow model = RoomTypeAllotmentMapper.ToPersistenceModel(allotment);

            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Sql.Update.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }
    }
}
