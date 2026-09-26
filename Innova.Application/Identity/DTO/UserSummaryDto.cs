namespace Innova.Application.Identity.DTO
{
    public sealed record UserSummaryDto(
        Guid UserId,
        string Username,
        string Role,
        bool IsActive );
}
