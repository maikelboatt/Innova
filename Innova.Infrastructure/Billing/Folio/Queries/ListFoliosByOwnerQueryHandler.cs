using System.Reflection;
using Dapper;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.DTO;
using Innova.Application.Billing.Folio.Queries;
using Innova.Infrastructure.Billing.Folio.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.Billing.Folio.Queries
{
    public sealed class ListFoliosByOwnerQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListFoliosByOwnerQuery, IReadOnlyCollection<FolioSummaryDto>>
    {
        private static readonly Assembly Assembly = typeof(ListFoliosByOwnerQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<FolioSummaryDto>> HandleAsync( ListFoliosByOwnerQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.ListByOwner.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    OwnerType = query.OwnerType.ToString(),
                    query.OwnerId
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            IEnumerable<FolioSummaryRow> rows = await connectionProvider.Connection.QueryAsync<FolioSummaryRow>(command);

            return FolioQueryHandlerMapper.MapToSummaryDto(rows);
        }
    }
}
