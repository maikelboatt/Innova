using Innova.Application.Exceptions;

namespace Innova.Application.Identity.Exceptions
{
    public sealed class UsernameAlreadyTakenException( string username )
        :ApplicationExceptions($"The username '{username}' is already taken.");
}
