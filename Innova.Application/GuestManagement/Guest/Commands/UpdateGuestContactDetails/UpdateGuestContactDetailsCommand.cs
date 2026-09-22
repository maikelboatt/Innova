using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.GuestManagement.Guest.Commands.UpdateGuestContactDetails
{
    public sealed record UpdateGuestContactDetailsCommand( Guid GuestId, string PhoneNumber, string? Email ):ICommand<Unit>;
}
