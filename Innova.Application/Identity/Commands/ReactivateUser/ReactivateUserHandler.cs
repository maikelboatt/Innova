using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Application.Identity.Exceptions;
using Innova.Domain.Identity.Aggregates;
using Innova.Domain.Identity.Repositories;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Identity.Commands.ReactivateUser
{
    public sealed class ReactivateUserHandler( IUserRepository userRepository, ICurrentUserContext currentUserContext )
        :ICommandHandler<ReactivateUserCommand, Unit>
    {
        public async Task<Unit> HandleAsync( ReactivateUserCommand command, CancellationToken ct = default )
        {

            UserId userId = UserId.From(command.UserId);

            User? user = await userRepository.GetByIdAsync(userId, ct)
                         ?? throw new UserNotFoundException(userId);

            user.Reactivate();

            await userRepository.UpdateAsync(user, ct);

            return Unit.Value;
        }
    }
}
