-- GetById.sql - Room Inventory

SELECT a.Id,
       a.RoomTypeId,
       a.[Date],
       a.TotalRooms,
       a.BookedCount,
       rtd.Name AS RoomTypeId,
       rtd.Name AS RoomTypeName
FROM roominventory.RoomTypeAllotment a
         INNER JOIN roominventory.RoomTypeDefinition rtd
                    ON a.RoomTypeId = rtd.Id
WHERE a.Id = @Id;