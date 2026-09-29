using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestUpdated( Guid GuestId, string PhoneNumber, string? Email ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GuestId;
        string IAuditableEvent.EntityType => "Guest";
        string IAuditableEvent.Summary => $"Guest {GuestId} has been successfully been updated at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTime.UtcNow;
    }
}
