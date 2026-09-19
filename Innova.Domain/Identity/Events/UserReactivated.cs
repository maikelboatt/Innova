using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserReactivated( Guid UserId, DateTime ReactivatedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
