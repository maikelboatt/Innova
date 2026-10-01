using Innova.Application.GuestManagement.Guest.DTO;
using Innova.Infrastructure.GuestManagement.Guest.Mapping;

namespace Innova.Infrastructure.GuestManagement.Guest.Queries
{
    public static class GuestQueryHandlerMapper
    {
        public static IReadOnlyCollection<GuestDto> MapToGuestDto( IEnumerable<GuestRow> guests ) => guests
                                                                                                     .Select(ToDto)
                                                                                                     .ToList()
                                                                                                     .AsReadOnly();

        public static GuestDto ToDto( GuestRow guestRow ) => new(
            guestRow.Id,
            guestRow.FirstName,
            guestRow.LastName,
            guestRow.MiddleName,
            DateOnly.FromDateTime(guestRow.DateOfBirth),
            guestRow.PhoneNumber,
            guestRow.Email,
            guestRow.IdentityDocumentType,
            guestRow.IdentityDocumentNumber,
            guestRow.IsActive,
            guestRow.CreatedAt
        );
    }
}
