using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.GuestManagement.Guest.Commands.ReactivateGuest
{
    public record ReactivateGuestCommand( Guid GuestId ):ICommand<Unit>;
}
