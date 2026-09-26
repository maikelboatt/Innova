using Innova.Application.Abstractions.Services;
using Innova.Application.Billing.Folio;
using Innova.Application.Billing.Folio.Exceptions;
using Innova.Application.FrontDesk.Stay.Exceptions;
using Innova.Domain.Billing.Aggregates;
using Innova.Domain.Billing.Repositories;
using Innova.Domain.Billing.ValueObjects;
using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class BillingAssemblyService( IFolioRepository folioRepository ):IBillingAssemblyService
    {
        public async Task<Folio> OpenFolioAsync( FolioOwner owner, string currency, CancellationToken ct = default )
        {
            Folio folio = Folio.Open(owner, currency);

            await folioRepository.SaveAsync(folio, ct);

            return folio;
        }

        public async Task<Folio> PostChargeAsync( Guid folioId,
                                                  Money amount,
                                                  string category,
                                                  string description,
                                                  CancellationToken ct = default )
        {
            Folio folio = await GetByIdAsync(folioId, ct);

            folio.PostCharge(Charge.Of(amount, category, description));

            await folioRepository.UpdateAsync(folio, ct);

            return folio;
        }

        public async Task<Folio> PostPaymentAsync( Guid folioId,
                                                   Money amount,
                                                   PaymentMethod method,
                                                   string? reference,
                                                   CancellationToken ct = default )
        {
            Folio folio = await GetByIdAsync(folioId, ct);

            folio.RecordPayment(Payment.Of(amount, method, reference));

            await folioRepository.UpdateAsync(folio, ct);

            return folio;
        }

        public async Task<Folio> PostAdjustmentAsync( Guid folioId,
                                                      Money amount,
                                                      AdjustmentType type,
                                                      string reason,
                                                      CancellationToken ct = default )
        {
            Folio folio = await GetByIdAsync(folioId, ct);

            folio.PostAdjustment(Adjustment.Of(amount, type, reason));

            await folioRepository.UpdateAsync(folio, ct);

            return folio;
        }

        public async Task<Folio> PostGuestFeeAsync( Guid guestId,
                                                    Money feeAmount,
                                                    string category,
                                                    string description,
                                                    CancellationToken ct = default )
        {
            Folio folio = Folio.Open(FolioOwner.ForGuest(guestId), feeAmount.Currency);

            folio.PostCharge(
                Charge.Of(feeAmount, category, description));

            await folioRepository.SaveAsync(folio, ct);

            return folio;
        }

        public async Task<Money> GetTotalOutstandingBalanceAsync( Guid stayId,
                                                                  IReadOnlyCollection<Guid> occupantGuestIds,
                                                                  Guid? groupBookingId,
                                                                  string currency,
                                                                  CancellationToken ct = default )
        {
            List<Folio> folios = await GetRelevantFoliosAsync(
                                     stayId,
                                     occupantGuestIds,
                                     groupBookingId,
                                     ct);

            return folios.Aggregate(Money.Zero(currency), ( acc, f ) => acc.Add(f.Balance));
        }

        public async Task<FolioSettlementResult> SettleAllForStayAsync( Guid stayId,
                                                                        IReadOnlyCollection<Guid> occupantGuestIds,
                                                                        Guid? groupBookingId,
                                                                        string currency,
                                                                        CancellationToken ct = default )
        {
            List<Folio> folios = await GetRelevantFoliosAsync(
                                     stayId,
                                     occupantGuestIds,
                                     groupBookingId,
                                     ct);

            Folio? unsettled = folios.FirstOrDefault(f => f.Balance.Amount != 0);
            if (unsettled is not null)
                throw new OutstandingBalanceException(StayId.From(stayId), unsettled.Balance);

            foreach (Folio folio in folios)
                folio.Settle();

            foreach (Folio folio in folios)
                await folioRepository.UpdateAsync(folio, ct);

            return new FolioSettlementResult(
                Money.Zero(currency),
                [
                    .. folios
                        .Select(f => f.Id.Value)
                ]);
        }

        public async Task<Folio> SettleAsync( Guid folioId, CancellationToken ct = default )
        {
            Folio folio = await GetByIdAsync(folioId, ct);

            folio.Settle();

            await folioRepository.UpdateAsync(folio, ct);

            return folio;
        }

        public async Task<Folio> VoidAsync( Guid folioId, CancellationToken ct = default )
        {
            Folio folio = await GetByIdAsync(folioId, ct);

            folio.Void();

            await folioRepository.UpdateAsync(folio, ct);

            return folio;
        }

        private async Task<Folio> GetByIdAsync( Guid folioId, CancellationToken ct )
        {
            FolioId id = FolioId.From(folioId);

            return await folioRepository.GetByIdAsync(id, ct) ?? throw new FolioNotFoundException(id);
        }

        private async Task<List<Folio>> GetRelevantFoliosAsync( Guid stayId,
                                                                IReadOnlyCollection<Guid> occupantGuestIds,
                                                                Guid? groupBookingId,
                                                                CancellationToken ct )
        {
            List<Folio> folios =
            [

                .. await folioRepository.GetByOwnerAsync(
                       FolioOwnerType.Stay,
                       stayId,
                       ct)
            ];

            foreach (Guid guestId in occupantGuestIds)
            {
                folios.AddRange(
                    await folioRepository.GetByOwnerAsync(
                        FolioOwnerType.GuestWithinStay,
                        guestId,
                        ct));
            }

            if (groupBookingId is not null)
            {
                folios.AddRange(
                    await folioRepository.GetByOwnerAsync(
                        FolioOwnerType.GroupBooking,
                        groupBookingId.Value,
                        ct));
            }

            return folios;
        }
    }
}
