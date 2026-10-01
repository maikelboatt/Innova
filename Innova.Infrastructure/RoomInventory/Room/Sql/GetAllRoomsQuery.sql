-- GetAllRoomsQuery.sql - Room Inventory - Room
SELECT r.Id,
       r.RoomNumber,
       r.FloorLevel,
       r.FloorWing,
       r.RoomTypeId,
       r.Status
FROM roominventory.Room r
ORDER BY r.FloorLevel,
         r.RoomNumber;