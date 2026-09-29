using Innova.Application.Abstractions.Events;
using Innova.Application.Shared.Abstractions;
using Innova.Domain.Common;

namespace Innova.Application.Dispatchers
{
    /// <summary>
    ///     Decorates the real dispatcher — same shape as
    ///     Logging/Authorization/Validation/TransactionCommandHandler
    ///     decorating ICommandHandler. Audits every dispatched event that
    ///     implements IAuditableEvent, then delegates to the inner
    ///     dispatcher for actual handler resolution/invocation. Adding
    ///     auditing to a new event means implementing three expression-
    ///     bodied members on that event's record — not writing a new class.
    /// </summary>
    public sealed class AuditingDomainEventDispatcher( IDomainEventDispatcher inner, IAuditLogger auditLogger, ILog )
        :IDomainEventDispatcher
    {
        public async Task DispatchAsync( IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default )
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                if (domainEvent is IAuditableEvent auditable)
                {
                    await auditLogger.LogAsync(
                        domainEvent.GetType()
                                   .Name,
                        auditable.EntityType,
                        auditable.EntityId,
                        auditable.Summary,
                        ct);
                }
            }

            await inner.DispatchAsync(domainEvents, ct);
        }
    }
}
