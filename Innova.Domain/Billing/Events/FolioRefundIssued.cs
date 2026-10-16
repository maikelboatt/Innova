using Innova.Domain.Common;

namespace Innova.Domain.Billing.Events
{
    public sealed record FolioRefundIssued(
        Guid FolioId,
        Guid FolioOwner,
        string OwnerType,
        decimal Amount,
        string Currency,
        string Method,
        string? Reference,
        string Reason,
        DateTime RefundedAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => FolioId;
        string IAuditableEvent.EntityType => "Folio";
        string IAuditableEvent.Summary => $"Folio with  {FolioId} has successfully refunded an amount of {Amount} at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
