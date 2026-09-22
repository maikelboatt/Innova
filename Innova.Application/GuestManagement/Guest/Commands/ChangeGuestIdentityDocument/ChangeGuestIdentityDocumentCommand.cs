using Innova.Application.Abstractions.Messaging;
using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Application.GuestManagement.Guest.Commands.ChangeGuestIdentityDocument
{
    public sealed record ChangeGuestIdentityDocumentCommand( Guid GuestId, IdentityDocumentType IdentityDocumentType, string IdentityDocumentNumber )
        :ICommand<Unit>;
}
