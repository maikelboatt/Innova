using Innova.Domain.Common;

namespace Innova.Domain.Billing.Events
{
    public sealed record FolioOpened(
        Guid FolioId,
        Guid FolioOwner,
        string OwnerType,
        string Currency ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
