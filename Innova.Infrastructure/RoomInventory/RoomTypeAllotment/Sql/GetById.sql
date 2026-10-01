-- GetById.sql - Room Inventory

SELECT a.Id,
       a.RoomTypeId,
       a.[Date],
       a.TotalRooms,
       a.BookedCount
FROM roominventory.RoomTypeAllotment a
WHERE a.Id = @Id;