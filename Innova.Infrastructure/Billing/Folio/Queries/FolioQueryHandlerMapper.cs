using Innova.Application.Billing.Folio.DTO;
using Innova.Infrastructure.Billing.Folio.Mapping;

namespace Innova.Infrastructure.Billing.Folio.Queries
{
    public static class FolioQueryHandlerMapper
    {
        public static FolioDto ToDto( FolioRow folio,
                                      IEnumerable<ChargeRow> charges,
                                      IEnumerable<PaymentRow> payments,
                                      IEnumerable<AdjustmentRow> adjustments )
        {
            IReadOnlyCollection<ChargeDto> chargeDtos = charges
                                                        .Select(charge => new ChargeDto(
                                                                    charge.Amount,
                                                                    charge.Currency,
                                                                    charge.Category,
                                                                    charge.Description,
                                                                    charge.PostedAt))
                                                        .ToList()
                                                        .AsReadOnly();

            IReadOnlyCollection<PaymentDto> paymentDtos = payments
                                                          .Select(payment => new PaymentDto(
                                                                      payment.Amount,
                                                                      payment.Currency,
                                                                      payment.Method,
                                                                      payment.Reference,
                                                                      payment.ReceivedAt))
                                                          .ToList()
                                                          .AsReadOnly();

            IReadOnlyCollection<AdjustmentDto> adjustmentDtos = adjustments
                                                                .Select(adjustment => new AdjustmentDto(
                                                                            adjustment.Amount,
                                                                            adjustment.Currency,
                                                                            adjustment.Type,
                                                                            adjustment.Reason,
                                                                            adjustment.PostedAt))
                                                                .ToList()
                                                                .AsReadOnly();

            decimal balance = charges.Sum(x => x.Amount) - payments.Sum(x => x.Amount) - adjustments.Sum(x => x.Amount);

            return new FolioDto(
                folio.Id,
                folio.OwnerType,
                folio.OwnerId,
                folio.Currency,
                folio.Status,
                balance,
                chargeDtos,
                paymentDtos,
                adjustmentDtos);
        }

        public static FolioSummaryDto ToSummaryDto(
            FolioSummaryRow row ) => new(
            row.Id,
            row.OwnerType,
            row.Status,
            row.Balance,
            row.Currency);

        public static IReadOnlyCollection<FolioSummaryDto> MapToSummaryDto(
            IEnumerable<FolioSummaryRow> rows ) => rows
                                                   .Select(ToSummaryDto)
                                                   .ToList()
                                                   .AsReadOnly();
    }
}
