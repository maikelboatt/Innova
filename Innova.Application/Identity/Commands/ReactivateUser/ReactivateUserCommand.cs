using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Identity.Commands.ReactivateUser
{
    public record ReactivateUserCommand( Guid UserId ):ICommand;
}
