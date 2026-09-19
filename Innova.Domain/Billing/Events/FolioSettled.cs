using Innova.Domain.Common;

namespace Innova.Domain.Billing.Events
{
    public sealed record FolioSettled(
        Guid FolioId,
        Guid FolioOwner,
        string OwnerType,
        string Currency,
        DateTime SettledAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
