-- Update.sql - Room Inventory

UPDATE roominventory.RoomTypeAllotment
SET TotalRooms  = @TotalRooms,
    BookedCount = @BookedCount
WHERE Id = @Id;