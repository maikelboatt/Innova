using Application.Exceptions;

namespace Innova.Application.Exceptions
{
    public class UnauthorizedCommandException( string message ):ApplicationExceptions(message);
}
