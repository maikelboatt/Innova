using System.Reflection;
using Dapper;
using Innova.Domain.Billing.Repositories;
using Innova.Domain.Billing.ValueObjects;
using Innova.Infrastructure.Billing.Folio.Mapping;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;

namespace Innova.Infrastructure.Billing.Folio.Repository
{
    public sealed class FolioRepository( IDbConnectionProvider connectionProvider ):IFolioRepository
    {
        private static readonly Assembly Assembly = typeof(FolioRepository).Assembly;

        public async Task<Domain.Billing.Aggregates.Folio?> GetByIdAsync( FolioId id, CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.GetById.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    FolioId = id.Value
                },
                connectionProvider.Transaction,
                cancellationToken: ct);

            using SqlMapper.GridReader multi = await connectionProvider.Connection.QueryMultipleAsync(command);

            FolioRow? folioRow = await multi.ReadSingleOrDefaultAsync<FolioRow>();

            if (folioRow is null)
                return null;

            IEnumerable<ChargeRow> charges = await multi.ReadAsync<ChargeRow>();

            IEnumerable<PaymentRow> payments = await multi.ReadAsync<PaymentRow>();

            IEnumerable<AdjustmentRow> adjustments = await multi.ReadAsync<AdjustmentRow>();

            return FolioMapper.ToDomain(
                folioRow,
                charges,
                payments,
                adjustments);
        }

        public async Task<IReadOnlyCollection<Domain.Billing.Aggregates.Folio>> GetByOwnerAsync( FolioOwnerType ownerType,
                                                                                                 Guid ownerId,
                                                                                                 CancellationToken ct = default )
        {
            string sql = SqlLoader.Load(Assembly, "Innova.Infrastructure.Billing.Folio.Sql.GetByOwner.sql");

            CommandDefinition command = new(
                sql,
                new
                {
                    OwnerType = ownerType.ToString(),
                    OwnerId = ownerId
                },
                connectionProvider.Transaction,
                cancellationToken: ct);

            using SqlMapper.GridReader multi = await connectionProvider.Connection.QueryMultipleAsync(command);

            List<FolioRow> folioRows = [.. await multi.ReadAsync<FolioRow>()];

            List<ChargeRow> chargeRows = [.. await multi.ReadAsync<ChargeRow>()];

            List<PaymentRow> paymentRows = [.. await multi.ReadAsync<PaymentRow>()];

            List<AdjustmentRow> adjustmentRows = [.. await multi.ReadAsync<AdjustmentRow>()];

            return folioRows
                   .Select(folio =>
                               FolioMapper.ToDomain(
                                   folio,
                                   chargeRows.Where(charge => charge.FolioId == folio.Id),
                                   paymentRows.Where(payment => payment.FolioId == folio.Id),
                                   adjustmentRows.Where(adjustment => adjustment.FolioId == folio.Id)))
                   .ToList()
                   .AsReadOnly();
        }

        public async Task SaveAsync( Domain.Billing.Aggregates.Folio folio, CancellationToken ct = default )
        {
            FolioRow row = FolioMapper.ToFolioRow(folio);

            await ExecuteAsync(
                "Innova.Infrastructure.Billing.Folio.Sql.Insert.sql",
                new
                {
                    row.Id,
                    row.OwnerType,
                    row.OwnerId,
                    row.Currency,
                    row.Status
                },
                ct);

            foreach (Charge charge in folio.Charges)
            {
                ChargeRow chargeRow =
                    FolioMapper.ToChargeRow(
                        folio,
                        charge);

                await ExecuteAsync(
                    "Innova.Infrastructure.Billing.Folio.Sql.InsertCharge.sql",
                    new
                    {
                        chargeRow.FolioId,
                        chargeRow.Amount,
                        chargeRow.Currency,
                        chargeRow.Category,
                        chargeRow.Description,
                        chargeRow.PostedAt
                    },
                    ct);
            }

            foreach (Payment payment in folio.Payments)
            {
                PaymentRow paymentRow =
                    FolioMapper.ToPaymentRow(
                        folio,
                        payment);

                await ExecuteAsync(
                    "Innova.Infrastructure.Billing.Folio.Sql.InsertPayment.sql",
                    new
                    {
                        paymentRow.FolioId,
                        paymentRow.Amount,
                        paymentRow.Currency,
                        paymentRow.Method,
                        paymentRow.Reference,
                        paymentRow.ReceivedAt
                    },
                    ct);
            }

            foreach (Adjustment adjustment in folio.Adjustments)
            {
                AdjustmentRow adjustmentRow =
                    FolioMapper.ToAdjustmentRow(
                        folio,
                        adjustment);

                await ExecuteAsync(
                    "Innova.Infrastructure.Billing.Folio.Sql.InsertAdjustment.sql",
                    new
                    {
                        adjustmentRow.FolioId,
                        adjustmentRow.Amount,
                        adjustmentRow.Currency,
                        adjustmentRow.Type,
                        adjustmentRow.Reason,
                        adjustmentRow.PostedAt
                    },
                    ct);
            }

        }

        public async Task UpdateAsync( Domain.Billing.Aggregates.Folio folio, CancellationToken ct = default )
        {
            FolioRow row = FolioMapper.ToFolioRow(folio);

            await ExecuteAsync(
                "Innova.Infrastructure.Billing.Folio.Sql.Update.sql",
                new
                {
                    row.Id,
                    row.Status
                },
                ct);

            await ExecuteAsync(
                "Innova.Infrastructure.Billing.Folio.Sql.DeleteCharges.sql",
                new
                {
                    FolioId = row.Id
                },
                ct);

            await ExecuteAsync(
                "Innova.Infrastructure.Billing.Folio.Sql.DeletePayments.sql",
                new
                {
                    FolioId = row.Id
                },
                ct);

            await ExecuteAsync(
                "Innova.Infrastructure.Billing.Folio.Sql.DeleteAdjustments.sql",
                new
                {
                    FolioId = row.Id
                },
                ct);

            foreach (Charge charge in folio.Charges)
            {
                ChargeRow chargeRow =
                    FolioMapper.ToChargeRow(
                        folio,
                        charge);

                await ExecuteAsync(
                    "Innova.Infrastructure.Billing.Folio.Sql.InsertCharge.sql",
                    new
                    {
                        chargeRow.FolioId,
                        chargeRow.Amount,
                        chargeRow.Currency,
                        chargeRow.Category,
                        chargeRow.Description,
                        chargeRow.PostedAt
                    },
                    ct);
            }

            foreach (Payment payment in folio.Payments)
            {
                PaymentRow paymentRow =
                    FolioMapper.ToPaymentRow(
                        folio,
                        payment);

                await ExecuteAsync(
                    "Innova.Infrastructure.Billing.Folio.Sql.InsertPayment.sql",
                    new
                    {
                        paymentRow.FolioId,
                        paymentRow.Amount,
                        paymentRow.Currency,
                        paymentRow.Method,
                        paymentRow.Reference,
                        paymentRow.ReceivedAt
                    },
                    ct);
            }

            foreach (Adjustment adjustment in folio.Adjustments)
            {
                AdjustmentRow adjustmentRow =
                    FolioMapper.ToAdjustmentRow(
                        folio,
                        adjustment);

                await ExecuteAsync(
                    "Innova.Infrastructure.Billing.Folio.Sql.InsertAdjustment.sql",
                    new
                    {
                        adjustmentRow.FolioId,
                        adjustmentRow.Amount,
                        adjustmentRow.Currency,
                        adjustmentRow.Type,
                        adjustmentRow.Reason,
                        adjustmentRow.PostedAt
                    },
                    ct);
            }
        }

        private async Task ExecuteAsync( string resourceName, object parameters, CancellationToken ct )
        {
            string sql = SqlLoader.Load(Assembly, resourceName);

            CommandDefinition command = new(
                sql,
                parameters,
                connectionProvider.Transaction,
                cancellationToken: ct);

            await connectionProvider.Connection.ExecuteAsync(command);
        }
    }
}
