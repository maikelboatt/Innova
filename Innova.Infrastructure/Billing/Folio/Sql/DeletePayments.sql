-- DeletePayments.sql - Billing

DELETE
FROM billing.Payment
WHERE FolioId = @FolioId;