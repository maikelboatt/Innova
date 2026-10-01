-- Insert.sql - Room Inventory

INSERT INTO roominventory.RoomTypeDefinition
(Id,
 Name,
 MaxOccupancy,
 BaseRateAmount,
 BaseRateCurrency)
VALUES (@Id,
        @Name,
        @MaxOccupancy,
        @BaseRateAmount,
        @BaseRateCurrency);