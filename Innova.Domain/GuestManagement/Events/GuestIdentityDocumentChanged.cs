using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestIdentityDocumentChanged( Guid GuestId, string DocumentType, string IdentityDocumentNumber ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GuestId;
        string IAuditableEvent.EntityType => "Guest";
        string IAuditableEvent.Summary => $"Guest  {GuestId} has successfully changed their identity document at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
