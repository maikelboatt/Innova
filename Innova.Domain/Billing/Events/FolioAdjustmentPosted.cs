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
        DateTime PostedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => FolioId;
        string IAuditableEvent.EntityType => "Folio";
        string IAuditableEvent.Summary => $"An adjustment of ({Currency} {Amount}) for Folio {FolioId} has successfully been made at {PostedAt:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
