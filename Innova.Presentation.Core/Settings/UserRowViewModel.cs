using Innova.Application.Identity.DTO;

namespace Innova.Presentation.Core.Settings
{
    public sealed class UserRowViewModel( UserSummaryDto dto, Guid currentUserId )
    {
        public Guid UserId { get; } = dto.UserId;
        public string Username { get; } = dto.Username;
        public string Role { get; } = dto.Role;
        public bool IsActive { get; } = dto.IsActive;

        public string StatusLabel => IsActive
                                         ? "Active"
                                         : "Deactivated";

        public bool IsCurrentUser { get; } = dto.UserId == currentUserId;
    }
}
