using Innova.Application.Abstractions.Events;
using Innova.Application.Shared.Abstractions;
using Innova.Domain.Reservations.Events;
using Microsoft.Extensions.Logging;

namespace Innova.Application.Reservations.Reservation.DomainEventHandlers
{
    // Audit/notification only now. The cancellation fee is posted
    // SYNCHRONOUSLY by ReservationService.CancelAsync directly (matching
    // the Housekeeping->Room precedent), not reactively here — if this
    // handler still called PostGuestFeeAsync, every cancellation would be
    // charged twice. FeeAmount/FeeCurrency stay on the event purely as
    // informational metadata for whatever else wants to react to a
    // cancellation (a notification email, reporting) without touching
    // billing again.
    public sealed class ReservationCancelledEventHandler(
        IAuditLogger auditLogger,
        ILogger<ReservationCancelledEventHandler> logger )
        :IDomainEventHandler<ReservationCancelled>
    {
        public async Task HandleAsync( ReservationCancelled @event, CancellationToken ct = default )
        {
            logger.LogInformation(
                "Reservation cancelled: {ReservationId} (fee of {FeeAmount} {FeeCurrency} already posted by ReservationService)",
                @event.ReservationId,
                @event.FeeAmount,
                @event.FeeCurrency);

            await auditLogger.LogAsync(
                "ReservationCancelledEventHandler",
                "Reservation",
                @event.ReservationId,
                $"Reservation {@event.ReservationId} cancelled at {@event.CancelledAt:g}.",
                ct);
        }
    }
}
