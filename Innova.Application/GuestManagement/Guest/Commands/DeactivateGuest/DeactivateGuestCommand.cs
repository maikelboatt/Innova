using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.GuestManagement.Guest.Commands.DeactivateGuest
{
    public sealed record DeactivateGuestCommand( Guid GuestId ):ICommand<Unit>;
}
