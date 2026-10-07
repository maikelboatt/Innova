using Innova.Application.Exceptions;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Exceptions
{
    public sealed class DuplicateGuestIdentityDocumentException( IdentityDocument identityDocument ):ApplicationExceptions(
        $"Guest with Identity document type '{identityDocument.Type}' and number '{identityDocument.Number}' already exists");
}
