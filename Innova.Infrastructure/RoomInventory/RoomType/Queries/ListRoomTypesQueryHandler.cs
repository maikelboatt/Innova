using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.RoomInventory.RoomType.Mapping;

namespace Innova.Infrastructure.RoomInventory.RoomType.Queries
{
    public sealed class ListRoomTypesQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListRoomTypesQuery, IReadOnlyCollection<RoomTypeDto>>
    {
        private static readonly Assembly Assembly = typeof(ListRoomTypesQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<RoomTypeDto>> HandleAsync( ListRoomTypesQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomType.Sql.ListRoomTypesQuery.sql");

            IEnumerable<RoomTypeRow> rows =
                await connectionProvider.Connection.QueryAsync<RoomTypeRow>(
                    new CommandDefinition(
                        sql,
                        transaction: connectionProvider.Transaction,
                        cancellationToken: ct));

            return RoomTypeQueryHandlerMapper.MapToRoomTypeDto(rows);
        }
    }
}
