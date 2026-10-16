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
    public sealed class GetFolioSummaryQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<GetFolioSummaryQuery, FolioListItemDto?>
    {
        private static readonly Assembly Assembly = typeof(GetFolioSummaryQueryHandler).Assembly;

        public async Task<FolioListItemDto?> HandleAsync( GetFolioSummaryQuery query, CancellationToken cancellationToken = default )
        {
            // Same SQL as the list, narrowed to one folio.
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.List.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    Status = (string?)null,
                    query.FolioId
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            FolioListItemRow? row = await connectionProvider.Connection.QuerySingleOrDefaultAsync<FolioListItemRow>(command);

            return row is null
                       ? null
                       : FolioListItemRow.ToDto(row);
        }
    }
}
