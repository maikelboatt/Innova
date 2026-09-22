using Innova.Domain.Identity.ValueObjects;

namespace Innova.Application.Abstractions.Services
{
    public interface ICurrentUserContext
    {
        Guid? UserId { get; }
        string? Username { get; }
        Role? Role { get; }
        bool IsAuthenticated { get; }

        void SignIn( Guid userId, string username, Role role );

        void SignOut();

        event Action? SignedOut;
    }
}
