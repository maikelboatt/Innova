using System.Reflection;
using Dapper;
using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.Repositories;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.RoomInventory.RoomType.Mapping;

namespace Innova.Infrastructure.RoomInventory.RoomType.Repository
{
    public sealed class RoomTypeDefinitionRepository( IDbConnectionProvider connectionProvider ):IRoomTypeDefinitionRepository
    {
        private static readonly Assembly Assembly = typeof(RoomTypeDefinitionRepository).Assembly;

        public async Task<RoomTypeDefinition?> GetByIdAsync( RoomTypeId id, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomType.Sql.GetById.sql");

            RoomTypeRow? typeRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<RoomTypeRow>(
                                       new CommandDefinition(
                                           sql,
                                           new
                                           {
                                               Id = id.Value
                                           },
                                           connectionProvider.Transaction,
                                           cancellationToken: ct));


            return typeRow is null
                       ? null
                       : RoomTypeMapper.ToDomain(typeRow);
        }


        public async Task<bool> ExistsByNameAsync( string name, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomType.Sql.ExistsByName.sql");

            return await connectionProvider.Connection.ExecuteScalarAsync<bool>(
                       new CommandDefinition(
                           sql,
                           new
                           {
                               Name = name
                           },
                           connectionProvider.Transaction,
                           cancellationToken: ct));
        }

        public async Task SaveAsync( RoomTypeDefinition room, CancellationToken ct = default )
        {
            RoomTypeRow model = RoomTypeMapper.ToPersistenceModel(room);
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomType.Sql.Insert.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }

        public async Task UpdateAsync( RoomTypeDefinition room, CancellationToken ct = default )
        {
            RoomTypeRow model = RoomTypeMapper.ToPersistenceModel(room);
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomType.Sql.Update.sql");

            CommandDefinition cmd = new(
                sql,
                model,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(cmd);
        }
    }
}
