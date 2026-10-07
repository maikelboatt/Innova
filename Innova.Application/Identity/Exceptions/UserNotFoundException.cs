using Innova.Application.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Identity.Exceptions
{
    public sealed class UserNotFoundException( UserId userId ):ApplicationExceptions($"User with ID {userId.Value} not found.");
}
