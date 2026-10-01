-- Update.sql - Guest Management
UPDATE guestmgmt.Guest
SET PhoneNumber            = @PhoneNumber,
    Email                  = @Email,
    IdentityDocumentType   =@IdentityDocumentType,
    IdentityDocumentNumber = @IdentityDocumentNumber,
    IsActive               = @IsActive
WHERE Id = @Id;