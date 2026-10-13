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
    public sealed class ListFolioOwnerOptionsQueryHandler( IDbConnectionProvider connectionProvider )
        :IQueryHandler<ListFolioOwnerOptionsQuery, IReadOnlyCollection<FolioOwnerOptionDto>>
    {
        private static readonly Assembly Assembly = typeof(ListFolioOwnerOptionsQueryHandler).Assembly;

        public async Task<IReadOnlyCollection<FolioOwnerOptionDto>> HandleAsync( ListFolioOwnerOptionsQuery query,
                                                                                 CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.OwnerOptions.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    query.OwnerType
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            IEnumerable<FolioOwnerOptionRow> rows = await connectionProvider.Connection.QueryAsync<FolioOwnerOptionRow>(command);

            return
            [
                .. rows.Select(row => new FolioOwnerOptionDto(
                                   row.OwnerId,
                                   row.Label,
                                   row.Detail,
                                   row.OpenFolioCount))
            ];
        }
    }
}
