using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserPasswordChanged( Guid UserId, DateTime ChangedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => UserId;
        string IAuditableEvent.EntityType => "User";
        string IAuditableEvent.Summary => $"Password for {UserId} has been changed successfully at {ChangedAt:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
