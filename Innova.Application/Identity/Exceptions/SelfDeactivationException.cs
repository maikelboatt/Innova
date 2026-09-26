using Application.Exceptions;

namespace Innova.Application.Identity.Exceptions
{
    public sealed class SelfDeactivationException( string message ):ApplicationExceptions(message);
}
