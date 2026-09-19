using Innova.Domain.Common;

namespace Innova.Domain.Billing.Events
{
    public sealed record FolioAdjustmentPosted(
        Guid FolioId,
        Guid FolioOwner,
        string OwnerType,
        decimal Amount,
        string Currency,
        string AdjustmentType,
        string Reason,
        DateTime PostedAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
