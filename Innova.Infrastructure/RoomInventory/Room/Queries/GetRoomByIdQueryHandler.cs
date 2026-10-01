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
    public sealed class GetRoomByIdQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetRoomByIdQuery, RoomDto?>
    {
        private static readonly Assembly Assembly = typeof(GetRoomByIdQueryHandler).Assembly;

        public async Task<RoomDto?> HandleAsync( GetRoomByIdQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.Room.Sql.GetRoomByIdQuery.sql");

            RoomRow? row =
                await connectionProvider.Connection.QuerySingleOrDefaultAsync<RoomRow>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            Id = query.RoomId
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));

            return row is null
                       ? null
                       : RoomQueryHandlerMapper.ToDto(row);
        }
    }
}
