using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Identity.Exceptions
{
    public sealed class UserDeactivateException( string message ):DomainException(message);
}
