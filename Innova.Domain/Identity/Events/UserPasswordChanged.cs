using Innova.Domain.Common;

namespace Innova.Domain.Identity.Events
{
    public sealed record UserPasswordChanged( Guid UserId, DateTime ChangedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
