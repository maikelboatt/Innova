using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.RoomInventory.Room.Mapping;

namespace Innova.Infrastructure.RoomInventory.Room.Queries
{
    public sealed class ListRoomsQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListRoomsQuery, IReadOnlyCollection<RoomDto>>
    {
        private static readonly Assembly Assembly = typeof(ListRoomsQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<RoomDto>> HandleAsync( ListRoomsQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.Room.Sql.ListRoomsQuery.sql");

            IEnumerable<RoomRow> rows =
                await connectionProvider.Connection.QueryAsync<RoomRow>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            query.Status,
                            query.RoomTypeId,
                            query.FloorLevel
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));

            return RoomQueryHandlerMapper.MapToRoomDto(rows);
        }
    }
}
