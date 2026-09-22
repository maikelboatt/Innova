using Innova.Application.Abstractions.Messaging;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Commands.RegisterGuest
{
    public sealed record RegisterGuestCommand(
        string FirstName,
        string LastName,
        string? MiddleName,
        DateOnly DateOfBirth,
        string PhoneNumber,
        string? Email,
        IdentityDocumentType IdentityDocumentType,
        string IdentityDocumentNumber ):ICommand<Guid>;
}
