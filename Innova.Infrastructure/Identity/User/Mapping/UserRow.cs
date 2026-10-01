namespace Innova.Infrastructure.Identity.User.Mapping
{
    public sealed class UserRow
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string PasswordHash { get; init; } = string.Empty;
        public string Role { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
