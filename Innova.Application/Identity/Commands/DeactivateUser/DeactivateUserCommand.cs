using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Identity.Commands.DeactivateUser
{
    public sealed record DeactivateUserCommand( Guid UserId ):ICommand;
}
