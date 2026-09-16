namespace Innova.Domain.Common
{
    public class AggregateRoot<TId>:Entity<TId>
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        protected AggregateRoot()
        {
        }

        protected AggregateRoot( TId newGuid ):base(newGuid)
        {
        }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent( IDomainEvent domainEvent )
        {
            ArgumentNullException.ThrowIfNull(domainEvent);
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
