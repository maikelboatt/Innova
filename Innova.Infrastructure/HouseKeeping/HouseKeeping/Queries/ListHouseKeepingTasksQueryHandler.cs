using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.HouseKeeping.HouseKeeping.DTO;
using Innova.Application.HouseKeeping.HouseKeeping.Queries;
using Innova.Infrastructure.HouseKeeping.HouseKeeping.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.HouseKeeping.HouseKeeping.Queries
{
    public sealed class ListHouseKeepingTasksQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListHouseKeepingTasksQuery, IReadOnlyCollection<HouseKeepingTaskSummaryDto>>
    {
        private static readonly Assembly Assembly = typeof(ListHouseKeepingTasksQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<HouseKeepingTaskSummaryDto>> HandleAsync( ListHouseKeepingTasksQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.HouseKeeping.HouseKeeping.Queries.Sql.ListHouseKeepingTasksQuery.sql");

            IEnumerable<HouseKeepingTaskSummaryRow> rows =
                await connectionProvider.Connection.QueryAsync<HouseKeepingTaskSummaryRow>(
                    new CommandDefinition(
                        sql,
                        new
                        {
                            query.Status,
                            query.AssignedStaffId,
                            query.RoomId
                        },
                        connectionProvider.Transaction,
                        cancellationToken: ct));

            return HouseKeepingTaskQueryHandlerMapper
                .MapToSummaryDto(rows);
        }
    }
}
