using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.Exceptions
{
    public sealed class GuestDeactivateException( string message ):DomainException(message);
}
