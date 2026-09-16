using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.Exceptions
{
    public sealed class GuestReactivateException( string message ):DomainException(message);
}
