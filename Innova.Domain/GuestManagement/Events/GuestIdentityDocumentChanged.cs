using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestIdentityDocumentChanged( Guid GuestId, string DocumentType, string IdentityDocumentNumber ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
