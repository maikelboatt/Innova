using Innova.Domain.Billing.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Infrastructure.Billing.Folio.Mapping
{
    public static class FolioMapper
    {
        public static FolioRow ToFolioRow( Domain.Billing.Aggregates.Folio folio ) => new()
                                                                                      {
                                                                                          Id = folio.Id.Value,
                                                                                          OwnerType = folio.Owner.Type.ToString(),
                                                                                          OwnerId = folio.Owner.OwnerId,
                                                                                          Currency = folio.Currency,
                                                                                          Status = folio.Status.Value
                                                                                      };

        public static ChargeRow ToChargeRow( Domain.Billing.Aggregates.Folio folio, Charge charge ) => new()
                                                                                                       {
                                                                                                           FolioId = folio.Id.Value,
                                                                                                           Amount = charge.Amount.Amount,
                                                                                                           Currency = charge.Amount.Currency,
                                                                                                           Category = charge.Category,
                                                                                                           Description = charge.Description,
                                                                                                           PostedAt = charge.PostedAt
                                                                                                       };

        public static PaymentRow ToPaymentRow( Domain.Billing.Aggregates.Folio folio, Payment payment ) => new()
            {
                FolioId = folio.Id.Value,
                Amount = payment.Amount.Amount,
                Currency = payment.Amount.Currency,
                Method = payment.PaymentMethod.ToString(),
                Reference = payment.Reference,
                ReceivedAt = payment.ReceivedAt
            };

        public static AdjustmentRow ToAdjustmentRow( Domain.Billing.Aggregates.Folio folio, Adjustment adjustment ) => new()
            {
                FolioId = folio.Id.Value,
                Amount = adjustment.Amount.Amount,
                Currency = adjustment.Amount.Currency,
                Type = adjustment.Type.ToString(),
                Reason = adjustment.Reason,
                PostedAt = adjustment.PostedAt
            };

        public static Domain.Billing.Aggregates.Folio ToDomain( FolioRow folioRow,
                                                                IEnumerable<ChargeRow> chargeRows,
                                                                IEnumerable<PaymentRow> paymentRows,
                                                                IEnumerable<AdjustmentRow> adjustmentRows )
        {
            FolioOwner owner = FolioOwner.Of(Enum.Parse<FolioOwnerType>(folioRow.OwnerType), folioRow.OwnerId);

            IEnumerable<Charge> charges = chargeRows.Select(ToDomain);
            IEnumerable<Payment> payments = paymentRows.Select(ToDomain);
            IEnumerable<Adjustment> adjustments = adjustmentRows.Select(ToDomain);

            return Domain.Billing.Aggregates.Folio.Reconstitute(
                FolioId.From(folioRow.Id),
                owner,
                folioRow.Currency,
                MapStringToStatus(folioRow.Status),
                charges,
                payments,
                adjustments);
        }


        public static Charge ToDomain( ChargeRow row ) => Charge.Of(
            Money.Of(row.Amount, row.Currency),
            row.Category,
            row.Description);

        public static Payment ToDomain( PaymentRow row ) => Payment.Of(
            Money.Of(
                row.Amount,
                row.Currency),
            Enum.Parse<PaymentMethod>(row.Method),
            row.Reference
        );

        public static Adjustment ToDomain( AdjustmentRow row ) => Adjustment.Of(
            Money.Of(
                row.Amount,
                row.Currency),
            Enum.Parse<AdjustmentType>(row.Type),
            row.Reason);


        private static FolioStatus MapStringToStatus( string status ) => status switch
                                                                         {
                                                                             "Open"    => FolioStatus.Open,
                                                                             "Settled" => FolioStatus.Settled,
                                                                             "Void"    => FolioStatus.Void,
                                                                             _         => throw new ArgumentException($"Unknown folio status: {status}")
                                                                         };
    }
}
