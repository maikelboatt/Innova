-- GetRoomByIdQuery.sql - Room Inventory - Room

SELECT r.Id,
       r.RoomNumber,
       r.FloorLevel,
       r.FloorWing,
       r.RoomTypeId,
       r.Status,
       rtd.Name AS RoomTypeName
FROM roominventory.Room r
         INNER JOIN roominventory.RoomTypeDefinition rtd
                    ON r.RoomTypeId = rtd.Id
WHERE r.Id = @Id;