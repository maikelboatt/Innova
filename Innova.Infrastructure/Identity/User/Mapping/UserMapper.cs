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

        public static Domain.Identity.Aggregates.User ToDomain( UserRow row ) => Domain.Identity.Aggregates.User.Reconstitute(
            UserId.From(row.Id),
            row.Username,
            row.PasswordHash,
            Role.From(row.Role),
            row.IsActive);
    }
}
