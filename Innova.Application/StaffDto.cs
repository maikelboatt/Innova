namespace Innova.Application
{
    public sealed record StaffDto(
        Guid StaffId,
        string FirstName,
        string LastName,
        string? MiddleName,
        DateOnly DateOfBirth,
        string PhoneNumber,
        string Address,
        string? Email,
        bool IsActive,
        DateTime CreatedAt );
}
