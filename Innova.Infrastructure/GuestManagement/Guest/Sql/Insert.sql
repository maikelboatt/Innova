-- Insert.sql - Guest Management

INSERT INTO guestmgmt.Guest (Id, FirstName, LastName, MiddleName, DateOfBirth, PhoneNumber, Email, IdentityDocumentType,
                             IdentityDocumentNumber)
VALUES (@Id, @FirstName, @LastName, @MiddleName, @DateOfBirth,
        @PhoneNumber, @Email, @IdentityDocumentType,
        @IdentityDocumentNumber);