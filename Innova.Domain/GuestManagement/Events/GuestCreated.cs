using Innova.Domain.Common;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestCreated(
        Guid GuestId,
        string FirstName,
        string LastName,
        string? MiddleName,
        DateOnly DateOfBirth,
        string PhoneNumber,
        string? Email,
        string DocumentType,
        string IdentityDocumentNumber ):IDomainEvent, IAuditableEvent
    {
        Guid IAuditableEvent.EntityId => GuestId;
        string IAuditableEvent.EntityType => "Guest";
        string IAuditableEvent.Summary => $"Guest {FirstName} {LastName} | {GuestId} has been created successfully at {OccurredOn:g}.";
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
