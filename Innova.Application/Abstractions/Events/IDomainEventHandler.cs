using Innova.Domain.Common;

namespace Innova.Application.Abstractions.Events
{
    public interface IDomainEventHandler
    {
        Task HandleAsync( IDomainEvent @event, CancellationToken cancellationToken = default );
    }

    public interface IDomainEventHandler<in TEvent>:IDomainEventHandler where TEvent : IDomainEvent
    {
        Task HandleAsync( TEvent @event, CancellationToken cancellationToken = default );
    }
}
