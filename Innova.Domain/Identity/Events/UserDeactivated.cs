using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserDeactivated( Guid UserId, DateTime DeactivatedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
