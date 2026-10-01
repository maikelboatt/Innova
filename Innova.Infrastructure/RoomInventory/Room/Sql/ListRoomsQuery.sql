-- ListRoomsQuery.sql - Room Inventory - Room

SELECT r.Id,
       r.RoomNumber,
       r.FloorLevel,
       r.FloorWing,
       r.RoomTypeId,
       r.Status
FROM roominventory.Room r
WHERE (
    @Status IS NULL
        OR r.Status = @Status
    )
  AND (
    @RoomTypeId IS NULL
        OR r.RoomTypeId = @RoomTypeId
    )
  AND (
    @FloorLevel IS NULL
        OR r.FloorLevel = @FloorLevel
    )
ORDER BY r.FloorLevel,
         r.RoomNumber;