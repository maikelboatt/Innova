-- ExistByIdentityDocument.sql - Guest Management

SELECT CASE
           WHEN EXISTS
               (SELECT 1
                FROM guestmgmt.Guest
                WHERE IdentityDocumentType = @IdentityDocumentType
                  AND IdentityDocumentNumber = @IdentityDocumentNumber)
               THEN CAST(1 AS BIT)
           ELSE CAST(0 AS BIT)
           END;