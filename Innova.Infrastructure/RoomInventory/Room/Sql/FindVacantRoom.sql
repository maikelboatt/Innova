-- FindVacantRoom.sql - Room Inventory

SELECT TOP (1) r.Id,
               r.RoomNumber,
               r.FloorLevel,
               r.FloorWing,
               r.RoomTypeId,
               r.Status
FROM roominventory.Room r
WHERE r.RoomTypeId = @RoomTypeId
  AND r.Status = 'Vacant'
ORDER BY r.RoomNumber;