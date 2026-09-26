namespace Innova.Application.GuestManagement.Guest.DTO
{
    public sealed record GuestSummaryDto(
        Guid GuestId,
        string FullName,
        string PhoneNumber,
        bool IsActive );
}
