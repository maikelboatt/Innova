using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Application.Identity.Exceptions;
using Innova.Domain.Identity.Aggregates;
using Innova.Domain.Identity.Repositories;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Identity.Commands.DeactivateUser
{
    public sealed class DeactivateUserHandler( IUserRepository userRepository, ICurrentUserContext currentUserContext )
        :ICommandHandler<DeactivateUserCommand, Unit>
    {
        public async Task<Unit> HandleAsync( DeactivateUserCommand command, CancellationToken ct = default )
        {
            // Guard against self-deactivation — an Admin locking themselves
            // out mid-session (especially if they're the only signed-in
            // Admin) has no recovery path short of direct DB intervention.
            if (currentUserContext.UserId == command.UserId)
                throw new SelfDeactivationException("You cannot deactivate your own account.");

            UserId userId = UserId.From(command.UserId);

            User? user = await userRepository.GetByIdAsync(userId, ct)
                         ?? throw new UserNotFoundException(userId);

            user.Deactivate();

            await userRepository.UpdateAsync(user, ct);

            return Unit.Value;
        }
    }
}
