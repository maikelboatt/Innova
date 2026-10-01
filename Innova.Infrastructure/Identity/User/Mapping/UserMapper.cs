using Innova.Domain.Identity.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Infrastructure.Identity.User.Mapping
{
    public static class UserMapper
    {
        public static UserRow ToPersistenceModel( Domain.Identity.Aggregates.User user ) => new()
                                                                                            {
                                                                                                Id = user.UserId.Value,
                                                                                                Username = user.Username,
                                                                                                PasswordHash = user.PasswordHash,
                                                                                                Role = user.Role.Value,
                                                                                                IsActive = user.IsActive
                                                                                            };

        // No Create/Reconstitute split needed here — every field
        // (Username, PasswordHash, Role, IsActive) is a structural,
        // time-independent fact once persisted.
        // Nothing here is lifecycle-defaulted, so User.Reconstitute
        // trusting the row directly is correct, not a shortcut.
        public static Domain.Identity.Aggregates.User ToDomain( UserRow row ) => Domain.Identity.Aggregates.User.Reconstitute(
            UserId.From(row.Id),
            row.Username,
            row.PasswordHash,
            Role.From(row.Role),
            row.IsActive);
    }
}
