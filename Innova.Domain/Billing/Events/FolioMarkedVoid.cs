using Innova.Domain.Common;

namespace Innova.Domain.Billing.Events
{
    public sealed record FolioMarkedVoid(
        Guid FolioId,
        Guid FolioOwner,
        string OwnerType,
        string Currency,
        DateTime MarkedVoidAt ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
