using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestDeactivated( Guid GuestId ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GuestId;
        string IAuditableEvent.EntityType => "Guest";
        string IAuditableEvent.Summary => $"Guest  {GuestId} has successfully been deactivated at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
