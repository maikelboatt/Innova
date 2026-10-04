-- DeleteCharges.sql - Billing 
DELETE
FROM billing.Charge
WHERE FolioId = @FolioId;
