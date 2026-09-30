using Innova.Domain.Common;

namespace Innova.Domain.Billing.Events
{
    public sealed record FolioSettled(
        Guid FolioId,
        Guid FolioOwner,
        string OwnerType,
        string Currency,
        DateTime SettledAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => FolioId;
        string IAuditableEvent.EntityType => "Folio";
        string IAuditableEvent.Summary => $"Folio {FolioId} has successfully been settled at {SettledAt:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
