using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserReactivated( Guid UserId, DateTime ReactivatedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => UserId;
        string IAuditableEvent.EntityType => "User";
        string IAuditableEvent.Summary => $"User {UserId} has been reactivated successfully at {ReactivatedAt:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
