-- DeleteAdjustment.sql - Billing

DELETE
FROM billing.Adjustment
WHERE FolioId = @FolioId;