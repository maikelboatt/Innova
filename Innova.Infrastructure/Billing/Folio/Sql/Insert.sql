-- Insert.sql - Billing

INSERT INTO billing.Folio
(Id,
 OwnerType,
 OwnerId,
 Currency,
 Status)
VALUES (@Id,
        @OwnerType,
        @OwnerId,
        @Currency,
        @Status);