using System.Reflection;
using Innova.Application.Abstractions.Events;
using Innova.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Innova.Application.Dispatchers
{
    /// <summary>
    ///     Runs once during DI composition. For every known IDomainEvent type,
    ///     resolves every registered IDomainEventHandler&lt;TEvent&gt; and wraps
    ///     each in a strongly-typed closure via WrapHandler&lt;TEvent&gt; — a real
    ///     generic method, so the compiler generates the cast for every closed
    ///     type, unlike Type.GetMethod("HandleAsync") which had to guess at
    ///     runtime and threw AmbiguousMatchException. Any wiring mistake here
    ///     surfaces once at app boot, not intermittently on whichever request
    ///     happens to raise that event first.
    /// </summary>
    public static class DomainEventDispatcherBuilder
    {
        public static IReadOnlyDictionary<Type, IReadOnlyCollection<Func<IDomainEvent, CancellationToken, Task>>> Build(
            IServiceProvider serviceProvider,
            IEnumerable<Type> domainEventTypes )
        {
            Dictionary<Type, IReadOnlyCollection<Func<IDomainEvent, CancellationToken, Task>>> map = [];

            MethodInfo wrapMethodDefinition = typeof(DomainEventDispatcherBuilder)
                .GetMethod(nameof(WrapHandler), BindingFlags.NonPublic | BindingFlags.Static)!;

            foreach (Type eventType in domainEventTypes)
            {
                Type handlerInterfaceType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
                IEnumerable<object?> handlers = serviceProvider.GetServices(handlerInterfaceType);

                MethodInfo wrapMethod = wrapMethodDefinition.MakeGenericMethod(eventType);

                List<Func<IDomainEvent, CancellationToken, Task>> wrapped = handlers
                                                                            .Where(h => h is not null)
                                                                            .Select(h => (Func<IDomainEvent, CancellationToken, Task>)wrapMethod.Invoke(
                                                                                        null,
                                                                                        [h])!)
                                                                            .ToList();

                if (wrapped.Count > 0)
                    map[eventType] = wrapped;
            }

            return map;
        }

        // The only cast in this whole mechanism, written once, generically —
        // not duplicated per handler class the way the old delegate pattern
        // required.
        private static Func<IDomainEvent, CancellationToken, Task> WrapHandler<TEvent>( IDomainEventHandler<TEvent> handler )
            where TEvent : IDomainEvent => ( domainEvent, ct ) => handler.HandleAsync((TEvent)domainEvent, ct);
    }
}
