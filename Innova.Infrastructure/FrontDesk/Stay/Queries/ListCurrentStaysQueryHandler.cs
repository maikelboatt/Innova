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
    public sealed class ListCurrentStaysQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListCurrentStaysQuery, IReadOnlyCollection<StaySummaryDto>>
    {
        private static readonly Assembly Assembly = typeof(ListCurrentStaysQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<StaySummaryDto>> HandleAsync( ListCurrentStaysQuery query, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.FrontDesk.Stay.Sql.GetCurrentStays.sql");
            IEnumerable<StaySummaryRow> stayRow = await connectionProvider.Connection.QueryAsync<StaySummaryRow>(
                                                      new CommandDefinition(
                                                          sql,
                                                          new
                                                          {
                                                              CurrentDate = DateTime.UtcNow
                                                          },
                                                          connectionProvider.Transaction,
                                                          cancellationToken: ct));

            return StayQueryHandlerMapper
                   .ToSummaryDto(stayRow)
                   .ToList()
                   .AsReadOnly();
        }
    }
}
