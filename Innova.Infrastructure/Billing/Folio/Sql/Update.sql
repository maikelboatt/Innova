-- Update.sql - Billing 

UPDATE billing.Folio
SET Status = @Status
WHERE Id = @Id;