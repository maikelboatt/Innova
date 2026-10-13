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
    public sealed class ListFoliosQueryHandler( IDbConnectionProvider connectionProvider ):IQueryHandler<ListFoliosQuery, IReadOnlyCollection<FolioListItemDto>>
    {
        private static readonly Assembly Assembly = typeof(ListFoliosQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<FolioListItemDto>> HandleAsync( ListFoliosQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.List.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    query.Status
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            IEnumerable<FolioListItemRow> rows = await connectionProvider.Connection.QueryAsync<FolioListItemRow>(command);

            return
            [
                .. rows.Select(row => new FolioListItemDto(
                                   row.FolioId,
                                   row.OwnerType,
                                   row.OwnerId,
                                   row.OwnerName,
                                   row.OwnerDetail,
                                   row.Currency,
                                   row.Status,
                                   row.TotalCharges,
                                   row.TotalPayments,
                                   row.TotalAdjustments,
                                   row.Balance,
                                   row.LastActivityAt))
            ];
        }
    }
}
