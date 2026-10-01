-- GetByRoomTypeAndDate.sql
SELECT a.Id, a.RoomTypeId, a.[Date], a.TotalRooms, a.BookedCount
FROM roominventory.RoomTypeAllotment a
WITH (UPDLOCK, ROWLOCK)
WHERE a.RoomTypeId = @RoomTypeId
  AND a.[Date] = @Date;