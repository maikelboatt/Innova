using Application.Exceptions;

namespace Innova.Application.Identity.Exceptions
{
    public class InvalidCredentialsException():ApplicationExceptions("Invalid username or password.");
}
