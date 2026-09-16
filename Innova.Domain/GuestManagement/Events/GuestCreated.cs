using Innova.Domain.Common;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Domain.GuestManagement.Events
{
    public sealed record GuestCreated(
        GuestId GuestId,
        string FirstName,
        string LastName,
        string? MiddleName,
        DateOnly DateOfBirth,
        string PhoneNumber,
        string? Email,
        string DocumentType,
        string IdentityDocumentNumber ):IDomainEvent
    {
        public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
    }
}
