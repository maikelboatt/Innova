-- Update.sql - Room Inventory

UPDATE roominventory.RoomTypeDefinition
SET Name             = @Name,
    MaxOccupancy     = @MaxOccupancy,
    BaseRateAmount   = @BaseRateAmount,
    BaseRateCurrency = @BaseRateCurrency
WHERE Id = @Id;