using Innova.Domain.Common;
using Innova.Domain.Identity.Events;
using Innova.Domain.Identity.Exceptions;
using Innova.Domain.Identity.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Identity.Aggregates
{
    public sealed class User:AggregateRoot<UserId>
    {
        private User() { }

        private User( UserId userId,
                      string username,
                      string passwordHash,
                      Role role,
                      bool isActive )
            :base(userId)
        {
            UserId = userId;
            Username = username;
            PasswordHash = passwordHash;
            Role = role;
            IsActive = isActive;
        }

        public UserId UserId { get; }
        public string Username { get; }
        public string PasswordHash { get; private set; }
        public Role Role { get; private set; }
        public bool IsActive { get; private set; }

        public static User Register( string username, string passwordHash, Role role )
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is required.", nameof(username));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.", nameof(passwordHash));

            ArgumentNullException.ThrowIfNull(role);

            UserId userId = UserId.New();

            User user = new(
                userId,
                username.Trim(),
                passwordHash,
                role,
                true);

            user.RaiseDomainEvent(
                new UserRegistered(
                    userId.Value,
                    username.Trim(),
                    role.Value,
                    DateTime.UtcNow));

            return user;
        }

        public static User Reconstitute( UserId userId,
                                         string username,
                                         string passwordHash,
                                         Role role,
                                         bool isActive ) => new(
            userId,
            username,
            passwordHash,
            role,
            isActive);

        public void Deactivate()
        {
            if (!IsActive)
                throw new UserDeactivateException("User is already deactivated.");

            IsActive = false;
            RaiseDomainEvent(new UserDeactivated(UserId.Value, DateTime.UtcNow));
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new UserReactivateException("User is already active.");

            IsActive = true;
            RaiseDomainEvent(new UserReactivated(UserId.Value, DateTime.UtcNow));
        }

        public void ChangePasswordHash( string newPasswordHash )
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash is required.", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
            RaiseDomainEvent(new UserPasswordChanged(UserId.Value, DateTime.UtcNow));
        }
    }
}
