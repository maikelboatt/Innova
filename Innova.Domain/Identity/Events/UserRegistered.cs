using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserRegistered(
        Guid UserId,
        string Username,
        string Role,
        DateTime RegisteredAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
