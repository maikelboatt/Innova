using Innova.Domain.Common;
using Innova.Domain.GuestManagement.Events;
using Innova.Domain.GuestManagement.Exceptions;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Shared.Exceptions;

namespace Innova.Domain.GuestManagement.Aggregates
{
    public sealed class Guest:AggregateRoot<GuestId>
    {
        private Guest() { }

        private Guest( GuestId guestId,
                       PersonName personName,
                       DateOfBirth dateOfBirth,
                       ContactDetails contactDetails,
                       IdentityDocument identityDocument ):base(guestId)
        {
            PersonName = personName;
            DateOfBirth = dateOfBirth;
            ContactDetails = contactDetails;
            IdentityDocument = identityDocument;
            IsActive = true;
        }

        public PersonName PersonName { get; private set; }
        public DateOfBirth DateOfBirth { get; private set; }
        public ContactDetails ContactDetails { get; private set; }
        public IdentityDocument IdentityDocument { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public static Guest Create( PersonName personName,
                                    DateOfBirth dateOfBirth,
                                    ContactDetails contactDetails,
                                    IdentityDocument identityDocument )
        {
            ArgumentNullException.ThrowIfNull(personName);
            ArgumentNullException.ThrowIfNull(dateOfBirth);
            ArgumentNullException.ThrowIfNull(contactDetails);
            ArgumentNullException.ThrowIfNull(identityDocument);

            GuestId guestId = GuestId.New();

            Guest guest = new(
                guestId,
                personName,
                dateOfBirth,
                contactDetails,
                identityDocument);

            guest.RaiseDomainEvent(
                new GuestCreated(
                    guest.Id,
                    personName.FirstName,
                    personName.LastName,
                    personName.MiddleName,
                    dateOfBirth.Value,
                    contactDetails.PhoneNumber,
                    contactDetails.Email,
                    identityDocument.Type.ToString(),
                    identityDocument.Number));

            return guest;
        }

        public void UpdateContactDetails( ContactDetails newContactDetails )
        {
            ArgumentNullException.ThrowIfNull(newContactDetails);

            EnsureActive();

            ContactDetails = newContactDetails;

            RaiseDomainEvent(new GuestUpdated(Id.Value, newContactDetails.PhoneNumber, newContactDetails.Email));
        }

        public void ChangeIdentityDocument( IdentityDocument identityDocument )
        {
            ArgumentNullException.ThrowIfNull(identityDocument);

            EnsureActive();

            IdentityDocument = identityDocument;

            RaiseDomainEvent(new GuestIdentityDocumentChanged(Id.Value, identityDocument.Type.ToString(), identityDocument.Number));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new GuestDeactivateException("Guest is already inactive.");

            IsActive = false;

            RaiseDomainEvent(
                new GuestDeactivated(Id.Value));
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new GuestReactivateException("Guest is already active.");

            IsActive = true;

            RaiseDomainEvent(
                new GuestReactivated(Id.Value));
        }

        private void EnsureActive()
        {
            if (!IsActive) throw new DomainException("This operation cannot be performed on an inactive guest.");
        }

        public static Guest Reconstitute( GuestId guestId,
                                          PersonName personName,
                                          DateOfBirth dateOfBirth,
                                          ContactDetails contactDetails,
                                          IdentityDocument identityDocument,
                                          DateTime createdAt,
                                          DateTime updatedAt )
        {
            Guest guest = new(
                              guestId,
                              personName,
                              dateOfBirth,
                              contactDetails,
                              identityDocument)
                          {
                              CreatedAt = createdAt
                          };

            return guest;
        }
    }
}
