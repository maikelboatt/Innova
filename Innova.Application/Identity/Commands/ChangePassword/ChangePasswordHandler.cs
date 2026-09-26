using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Application.Exceptions;
using Innova.Application.Identity.Exceptions;
using Innova.Domain.Identity.Aggregates;
using Innova.Domain.Identity.Repositories;
using Innova.Domain.Shared.ValueObjects;
using MediCore.Application.Abstractions.Services;

namespace Innova.Application.Identity.Commands.ChangePassword
{
    public sealed class ChangePasswordHandler( IUserRepository userRepository, IPasswordHasher passwordHasher, ICurrentUserContext currentUserContext )
        :ICommandHandler<ChangePasswordCommand, Unit>
    {
        public async Task<Unit> HandleAsync( ChangePasswordCommand command, CancellationToken ct = default )
        {
            // Get the current user
            if (!currentUserContext.UserId.HasValue)
                throw new UnauthorizedCommandException(
                    "You must be signed in to change your password.");

            User? user = await userRepository.GetByIdAsync(UserId.From(currentUserContext.UserId.Value), ct);

            if (user is null || !passwordHasher.Verify(user.PasswordHash, command.OldPassword))
                throw new InvalidCredentialsException();

            // Hash the new password
            string newPasswordHash = passwordHasher.Hash(command.NewPassword);

            // Update the user's password hash
            user.ChangePasswordHash(newPasswordHash);
            await userRepository.UpdateAsync(user, ct);

            return Unit.Value;
        }
    }
}
