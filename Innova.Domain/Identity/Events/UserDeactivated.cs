using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserDeactivated( Guid UserId, DateTime DeactivatedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => UserId;
        string IAuditableEvent.EntityType => "User";
        string IAuditableEvent.Summary => $"User {UserId} has been deactivated successfully at {DeactivatedAt:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
