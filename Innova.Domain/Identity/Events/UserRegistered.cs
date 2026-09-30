using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserRegistered(
        Guid UserId,
        string Username,
        string Role,
        DateTime RegisteredAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => UserId;
        string IAuditableEvent.EntityType => "User";
        string IAuditableEvent.Summary => $"User {Username} | {UserId} has been registered successfully at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
