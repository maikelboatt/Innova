using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Exceptions
{
    public sealed class DuplicateGuestIdentityDocumentException( IdentityDocument identityDocument ):ApplicationException(
        $"Guest with Identity document type '{identityDocument.Type}' and number '{identityDocument.Number}' already exists");
}
