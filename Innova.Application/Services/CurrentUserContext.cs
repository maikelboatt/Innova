using Innova.Application.Abstractions.Services;
using Innova.Domain.Identity.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class CurrentUserContext:ICurrentUserContext
    {
        public Guid? UserId { get; private set; }
        public string? Username { get; private set; }
        public Role? Role { get; private set; }
        public bool IsAuthenticated => UserId is not null;

        public void SignIn( Guid userId, string username, Role role )
        {
            UserId = userId;
            Username = username;
            Role = role;
        }

        public void SignOut()
        {
            UserId = null;
            Username = null;
            Role = null;
            SignedOut?.Invoke();
        }

        public event Action? SignedOut;
    }
}
