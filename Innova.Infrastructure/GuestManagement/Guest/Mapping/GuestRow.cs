namespace Innova.Infrastructure.GuestManagement.Guest.Mapping
{
    public sealed class GuestRow
    {
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? MiddleName { get; init; } = string.Empty;
        public DateTime DateOfBirth { get; init; }
        public string PhoneNumber { get; init; } = string.Empty;
        public string? Email { get; init; } = string.Empty;
        public string IdentityDocumentType { get; init; } = string.Empty;
        public string IdentityDocumentNumber { get; init; } = string.Empty;
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
