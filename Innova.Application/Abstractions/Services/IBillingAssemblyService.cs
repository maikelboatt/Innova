using Innova.Application.Billing.Folio;
using Innova.Domain.Billing.Aggregates;
using Innova.Domain.Billing.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface IBillingAssemblyService
    {
        Task<Folio> OpenFolioAsync( FolioOwner owner, string currency, CancellationToken ct = default );

        Task<Folio> PostChargeAsync( Guid folioId,
                                     Money amount,
                                     string category,
                                     string description,
                                     CancellationToken ct = default );

        Task<Folio> PostPaymentAsync( Guid folioId,
                                      Money amount,
                                      PaymentMethod method,
                                      string? reference,
                                      CancellationToken ct = default );

        Task<Folio> PostAdjustmentAsync( Guid folioId,
                                         Money amount,
                                         AdjustmentType type,
                                         string reason,
                                         CancellationToken ct = default );

        // No Stay required — opens a fresh Guid-owned folio and charges the
        // fee directly. Used for reservations marked no shows and cancelled.
        Task<Folio> PostGuestFeeAsync( Guid guestId,
                                       Money feeAmount,
                                       string category,
                                       string description,
                                       CancellationToken ct = default );

        // The checkout balance check: every folio owned directly by the
        // Stay, every folio owned by any of its occupants (split billing),
        // and the group booking's folio if one exists — summed together.
        Task<Money> GetTotalOutstandingBalanceAsync(
            Guid stayId,
            IReadOnlyCollection<Guid> occupantGuestIds,
            Guid? groupBookingId,
            string currency,
            CancellationToken ct = default );

        Task<FolioSettlementResult> SettleAllForStayAsync(
            Guid stayId,
            IReadOnlyCollection<Guid> occupantGuestIds,
            Guid? groupBookingId,
            string currency,
            CancellationToken ct = default );

        Task<Folio> SettleAsync( Guid folioId, CancellationToken ct = default );

        Task<Folio> VoidAsync( Guid folioId, CancellationToken ct = default );
    }
}
