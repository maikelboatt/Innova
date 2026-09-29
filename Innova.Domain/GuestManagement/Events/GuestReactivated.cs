using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestReactivated( Guid GuestId ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GuestId;
        string IAuditableEvent.EntityType => "Guest";
        string IAuditableEvent.Summary => $"Guest  {GuestId} has successfully been reactivated at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
