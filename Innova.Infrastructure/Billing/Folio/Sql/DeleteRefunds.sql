-- DeleteRefunds.sql - Billing
-- Used by FolioRepository.UpdateAsync, which re-writes each ledger collection (delete, then re-insert).
DELETE
FROM billing.Refund
WHERE FolioId = @FolioId;