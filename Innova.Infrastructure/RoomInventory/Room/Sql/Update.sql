-- Update.sql - Room Inventory
UPDATE roominventory.Room
SET Status = @Status
WHERE Id = @Id;