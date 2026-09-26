using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.DTO
{
    public record GuestDto(
        Guid GuestId,
        string FirstName,
        string LastName,
        string? MiddleName,
        DateOnly DateOfBirth,
        string PhoneNumber,
        string? Email,
        IdentityDocumentType IdentityDocumentType,
        string IdentityDocumentNumber,
        bool IsActive,
        DateTime CreatedAt );
}
