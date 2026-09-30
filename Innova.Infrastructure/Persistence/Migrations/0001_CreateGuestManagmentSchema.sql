-- 0001_CreateGuestManagmentSchema.sql

CREATE SCHEMA guestmgmt;
GO

CREATE TABLE guestmgmt.Guest
(
    Id                     UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    FirstName              NVARCHAR(100)    NOT NULL,
    LastName               NVARCHAR(100)    NOT NULL,
    MiddleName             NVARCHAR(100)    NULL,
    DateOfBirth            DATE             NOT NULL,
    PhoneNumber            NVARCHAR(30)     NOT NULL,
    Email                  NVARCHAR(256)    NULL,
    IdentityDocumentType   NVARCHAR(30)     NOT NULL,
    IdentityDocumentNumber NVARCHAR(50)     NOT NULL,
    IsActive               BIT              NOT NULL DEFAULT 1,
    CreatedAt              DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_Guest_Identity UNIQUE (IdentityDocumentNumber, IdentityDocumentNumber),

    CONSTRAINT CK_IdentityDocument_Type
        CHECK (
            IdentityDocumentType IN (
                                     'Passport', 'NationalId', 'GhanaCard', 'DriversLicense', 'VotersId', 'Other'
                )
            )
);
GO

CREATE UNIQUE INDEX UX_Guest_IdentityDocument
    ON guestmgmt.Guest (IdentityDocumentType, IdentityDocumentNumber);
GO

CREATE INDEX IX_Guest_PhoneNumber
    ON guestmgmt.Guest (PhoneNumber);
GO