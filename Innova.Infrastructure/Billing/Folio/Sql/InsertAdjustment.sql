-- InsertAdjustment.sql - Billing

INSERT INTO billing.Adjustment
(FolioId,
 Amount,
 Currency,
 Type,
 Reason,
 PostedAt)
VALUES (@FolioId,
        @Amount,
        @Currency,
        @Type,
        @Reason,
        @PostedAt);