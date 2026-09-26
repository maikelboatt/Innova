using Innova.Application.Abstractions.Events;
using Innova.Application.Shared.Abstractions;
using Innova.Domain.Reservations.Events;
using Microsoft.Extensions.Logging;

namespace Innova.Application.Reservations.Reservation.DomainEventHandlers
{
    // Audit/notification only — see ReservationCancelledEventHandler for
    // why. The no-show fee is posted synchronously by
    // ReservationService.MarkNoShowAsync directly.
    public sealed class ReservationMarkedNoShowEventHandler(
        IAuditLogger auditLogger,
        ILogger<ReservationMarkedNoShowEventHandler> logger )
        :IDomainEventHandler<ReservationMarkedNoShow>
    {
        public async Task HandleAsync( ReservationMarkedNoShow @event, CancellationToken ct = default )
        {
            logger.LogInformation(
                "Reservation marked no-show: {ReservationId} (fee of {FeeAmount} {FeeCurrency} already posted by ReservationService)",
                @event.ReservationId,
                @event.FeeAmount,
                @event.FeeCurrency);

            await auditLogger.LogAsync(
                "ReservationMarkedNoShowEventHandler",
                "Reservation",
                @event.ReservationId,
                $"Reservation {@event.ReservationId} marked no-show at {@event.MarkedNoShowAt:g}.",
                ct);
        }
    }
}
