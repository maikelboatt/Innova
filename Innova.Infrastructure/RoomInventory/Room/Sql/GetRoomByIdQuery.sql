-- GetRoomByIdQuery.sql - Room Inventory - Room

SELECT r.Id,
       r.RoomNumber,
       r.FloorLevel,
       r.FloorWing,
       r.RoomTypeId,
       r.Status
FROM roominventory.Room r
WHERE r.Id = @Id;