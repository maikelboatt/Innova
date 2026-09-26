using Innova.Application.Abstractions.Messaging;
using Innova.Application.Identity.Exceptions;
using Innova.Domain.Identity.Aggregates;
using Innova.Domain.Identity.Repositories;
using Innova.Domain.Identity.ValueObjects;
using MediCore.Application.Abstractions.Services;

namespace Innova.Application.Identity.Commands.RegisterUser
{
    public sealed class RegisterUserHandler( IUserRepository userRepository, IPasswordHasher passwordHasher )
        :ICommandHandler<RegisterUserCommand, Guid>
    {
        public async Task<Guid> HandleAsync( RegisterUserCommand command, CancellationToken ct = default )
        {
            User? existing = await userRepository.GetByUsernameAsync(command.Username, ct);
            if (existing is not null)
                throw new UsernameAlreadyTakenException(command.Username);

            string passwordHash = passwordHasher.Hash(command.Password);
            Role role = Role.From(command.Role);

            User user = User.Register(command.Username, passwordHash, role);

            await userRepository.SaveAsync(user, ct);

            return user.UserId.Value;
        }
    }
}
