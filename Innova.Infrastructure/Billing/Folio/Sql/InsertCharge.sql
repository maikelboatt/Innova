-- InsertCharge.sql - Billing

INSERT INTO billing.Charge
(FolioId,
 Amount,
 Currency,
 Category,
 Description,
 PostedAt)
VALUES (@FolioId,
        @Amount,
        @Currency,
        @Category,
        @Description,
        @PostedAt);