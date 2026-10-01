-- GetAll.sql - Guest Management
SELECT g.Id,
       g.FirstName,
       g.LastName,
       g.MiddleName,
       g.DateOfBirth,
       g.PhoneNumber,
       g.Email,
       g.IdentityDocumentType,
       g.IdentityDocumentNumber,
       g.IsActive,
       g.CreatedAt
FROM guestmgmt.Guest g;