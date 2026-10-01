using Innova.Domain.GuestManagement.ValueObjects;

namespace Innova.Infrastructure.GuestManagement.Guest.Mapping
{
    public static class GuestMapper
    {
        public static GuestRow ToPersistenceModel( Domain.GuestManagement.Aggregates.Guest guest )
        {
            Guid guestId = guest.Id.Value;

            GuestRow row = new()
                           {
                               Id = guestId,
                               FirstName = guest.PersonName.FirstName,
                               LastName = guest.PersonName.LastName,
                               MiddleName = guest.PersonName.MiddleName,
                               DateOfBirth = guest.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue),
                               PhoneNumber = guest.ContactDetails.PhoneNumber,
                               Email = guest.ContactDetails.Email,
                               IdentityDocumentType = guest.IdentityDocument.Type.ToString(),
                               IdentityDocumentNumber = guest.IdentityDocument.Number,
                               IsActive = guest.IsActive
                           };

            return row;
        }

        public static IReadOnlyCollection<Domain.GuestManagement.Aggregates.Guest> ToDomain( IReadOnlyCollection<GuestRow> rows ) => rows
            .Select(ToDomain)
            .ToList()
            .AsReadOnly();

        public static Domain.GuestManagement.Aggregates.Guest ToDomain( GuestRow row ) => Domain.GuestManagement.Aggregates.Guest.Reconstitute(
            GuestId.From(row.Id),
            PersonName.Create(row.FirstName, row.LastName, row.MiddleName),
            DateOfBirth.Create(DateOnly.FromDateTime(row.DateOfBirth)),
            ContactDetails.Create(row.PhoneNumber, row.Email),
            IdentityDocument.Create(Enum.Parse<IdentityDocumentType>(row.IdentityDocumentType), row.IdentityDocumentNumber),
            row.IsActive,
            row.CreatedAt);
    }
}
