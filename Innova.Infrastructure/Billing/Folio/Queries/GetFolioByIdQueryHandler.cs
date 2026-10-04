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
    public sealed class GetFolioByIdQueryHandler( IDbConnectionProvider connectionProvider ):IQueryHandler<GetFolioByIdQuery, FolioDto?>
    {
        private static readonly Assembly Assembly = typeof(GetFolioByIdQueryHandler).Assembly;

        public async Task<FolioDto?> HandleAsync( GetFolioByIdQuery query, CancellationToken cancellationToken = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.GetById.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    query.FolioId
                },
                connectionProvider.Transaction,
                cancellationToken: cancellationToken);

            using SqlMapper.GridReader multi = await connectionProvider.Connection.QueryMultipleAsync(command);

            FolioRow? folioRow =
                await multi.ReadSingleOrDefaultAsync<FolioRow>();

            if (folioRow is null)
                return null;

            IEnumerable<ChargeRow> chargeRows = await multi.ReadAsync<ChargeRow>();

            IEnumerable<PaymentRow> paymentRows = await multi.ReadAsync<PaymentRow>();

            IEnumerable<AdjustmentRow> adjustmentRows = await multi.ReadAsync<AdjustmentRow>();

            return FolioQueryHandlerMapper.ToDto(
                folioRow,
                chargeRows,
                paymentRows,
                adjustmentRows);
        }
    }
}
