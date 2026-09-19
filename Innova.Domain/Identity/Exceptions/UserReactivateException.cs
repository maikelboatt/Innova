using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.Identity.Exceptions
{
    public sealed class UserReactivateException( string message ):DomainException(message);
}
