using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Application.Identity.Exceptions;
using Innova.Domain.Identity.Aggregates;
using Innova.Domain.Identity.Repositories;

namespace Innova.Application.Identity.Commands.AuthenticateUser
{
    public sealed class AuthenticateUserHandler( IUserRepository userRepository, IPasswordHasher passwordHasher, ICurrentUserContext currentUserContext )
        :ICommandHandler<AuthenticateUserCommand, Guid>
    {
        public async Task<Guid> HandleAsync( AuthenticateUserCommand userCommand, CancellationToken ct = default )
        {
            User? user = await userRepository.GetByUsernameAsync(userCommand.Username, ct);

            if (user is null || !user.IsActive || !passwordHasher.Verify(user.PasswordHash, userCommand.Password))
                throw new InvalidCredentialsException();

            currentUserContext.SignIn(user.UserId.Value, user.Username, user.Role);

            return user.UserId.Value;
        }
    }
}
