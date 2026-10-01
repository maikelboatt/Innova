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
    public sealed class GetRoomTypeByIdQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetRoomTypeByIdQuery, RoomTypeDto?>
    {
        private static readonly Assembly Assembly = typeof(GetRoomTypeByIdQueryHandler).Assembly;

        public async Task<RoomTypeDto?> HandleAsync( GetRoomTypeByIdQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.RoomInventory.RoomType.Sql.GetById.sql");

            RoomTypeRow? row =
                await connectionProvider.Connection.QuerySingleOrDefaultAsync<RoomTypeRow>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            query.RoomTypeId
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));

            return row is null
                       ? null
                       : RoomTypeQueryHandlerMapper.ToDto(row);
        }
    }
}
