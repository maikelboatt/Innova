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
    public sealed class GetStayByIdQueryHandler( IDbConnectionProvider connectionProvider ):IQueryHandler<GetStayByIdQuery, StayDto?>
    {
        private static readonly Assembly Assembly = typeof(GetStayByIdQueryHandler).Assembly;

        public async Task<StayDto?> HandleAsync( GetStayByIdQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.GetById.sql");
            string occupantsSql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.GetOccupantsByStayId.sql");

            StayRow? stayRow = await connectionProvider.Connection.QuerySingleOrDefaultAsync<StayRow>(
                                   new CommandDefinition(
                                       sql,
                                       new
                                       {
                                           Id = query.StayId
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
                                                                    query.StayId
                                                                },
                                                                connectionProvider.Transaction,
                                                                cancellationToken: cancellationToken));

            return StayQueryHandlerMapper.ToDto(stayRow, occupantsRow);
        }
    }
}
