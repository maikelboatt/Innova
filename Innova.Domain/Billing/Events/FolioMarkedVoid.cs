using Innova.Domain.Common;

namespace Innova.Domain.Billing.Events
{
    public sealed record FolioMarkedVoid(
        Guid FolioId,
        Guid FolioOwner,
        string OwnerType,
        string Currency,
        DateTime MarkedVoidAt ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => FolioId;
        string IAuditableEvent.EntityType => "Folio";
        string IAuditableEvent.Summary => $"Folio {FolioId} has successfully been marked void at {MarkedVoidAt:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
