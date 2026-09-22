using Innova.Domain.Common;

namespace Innova.Application.Abstractions.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(
            IReadOnlyCollection<IDomainEvent> domainEvents,
            CancellationToken ct = default );
    }
}
