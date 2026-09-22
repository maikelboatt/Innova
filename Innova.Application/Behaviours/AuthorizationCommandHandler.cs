using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Application.Exceptions;
using Innova.Domain.Identity.Aggregates;
using Innova.Domain.Identity.Repositories;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Behaviours
{
    public sealed class AuthorizationCommandHandler<TCommand, TResult>(
        ICommandHandler<TCommand, TResult> inner,
        IUserRepository userRepository,
        ICurrentUserContext currentUser )
        :ICommandHandler<TCommand, TResult>
        where TCommand : ICommand<TResult>
    {
        public async Task<TResult> HandleAsync( TCommand command, CancellationToken ct = default )
        {
            if (command is IAllowAnonymousCommand)
                return await inner.HandleAsync(command, ct);

            if (!currentUser.IsAuthenticated)
                throw new UnauthorizedCommandException("You must be signed in to perform this action.");

            // if (CommandRoleRequirements.Map.TryGetValue(typeof(TCommand), out IReadOnlyCollection<Role>? allowedRoles)
            //     && !allowedRoles.Contains(currentUser.Role!))
            // {
            //     throw new UnauthorizedCommandException(
            //         $"Your role ({currentUser.Role}) is not permitted to perform this action.");
            // }

            UserId userId = UserId.From(currentUser.UserId!.Value);

            User? liveUser = await userRepository.GetByIdAsync(userId, ct);

            if (liveUser is not null && liveUser.IsActive) return await inner.HandleAsync(command, ct);
            currentUser.SignOut();
            throw new UnauthorizedCommandException("Your account has been deactivated. Please contact an administrator.");
        }
    }
}
