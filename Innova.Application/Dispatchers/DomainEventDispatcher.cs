using Innova.Application.Abstractions.Events;
using Innova.Domain.Common;

namespace Innova.Application.Dispatchers
{
    /// <summary>
    ///     Dispatches via delegates built once at startup (see
    ///     DomainEventDispatcherBuilder), not via per-dispatch reflection.
    ///     Handlers implement ONLY IDomainEventHandler&lt;TEvent&gt; — no
    ///     second, non-generic method required, because nothing here ever
    ///     calls through the non-generic interface.
    /// </summary>
    public sealed class DomainEventDispatcher( IReadOnlyDictionary<Type, IReadOnlyCollection<Func<IDomainEvent, CancellationToken, Task>>> handlersByEventType )
        :IDomainEventDispatcher
    {
        public async Task DispatchAsync( IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default )
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                if (!handlersByEventType.TryGetValue(domainEvent.GetType(), out IReadOnlyCollection<Func<IDomainEvent, CancellationToken, Task>>? handlers))
                    continue;

                foreach (Func<IDomainEvent, CancellationToken, Task> handler in handlers)
                    await handler(domainEvent, ct);
            }
        }
    }
}
