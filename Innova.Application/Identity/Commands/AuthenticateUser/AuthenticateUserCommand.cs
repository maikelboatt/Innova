using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Identity.Commands.AuthenticateUser
{
    public sealed record AuthenticateUserCommand( string Username, string Password )
        :ICommand<Guid>, IAllowAnonymousCommand;
}
