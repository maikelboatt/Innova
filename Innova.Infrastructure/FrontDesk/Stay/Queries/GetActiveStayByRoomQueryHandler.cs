using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.FrontDesk.Stay.DTO;
using Innova.Application.FrontDesk.Stay.Queries;
using Innova.Infrastructure.FrontDesk.Stay.Mapper;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.FrontDesk.Stay.Queries
{
    public sealed class GetActiveStayByRoomQueryHandler( IDbConnectionProvider connectionProvider ):IQueryHandler<GetActiveStayByRoomQuery, StayDto?>
    {
        private static readonly Assembly Assembly = typeof(GetActiveStayByRoomQueryHandler).Assembly;

        public async Task<StayDto?> HandleAsync( GetActiveStayByRoomQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.GetActiveStayByRoom.sql");
            string occupantsSql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.GetOccupantsByStayId.sql");

            StayRow? stayRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<StayRow>(
                                   new CommandDefinition(
                                       sql,
                                       new
                                       {
                                           query.RoomId
                                       },
                                       connectionProvider.Transaction,
                                       cancellationToken: cancellationToken));

            if (stayRow is null)
                return null;

            IEnumerable<StayOccupantRow> occupantsRow = await connectionProvider.Connection.QueryAsync<StayOccupantRow>(
                                                            new CommandDefinition(
                                                                occupantsSql,
                                                                new
                                                                {
                                                                    StayId = stayRow.Id
                                                                },
                                                                connectionProvider.Transaction,
                                                                cancellationToken: cancellationToken));

            return StayQueryHandlerMapper.ToDto(stayRow, occupantsRow);
        }
    }
}
