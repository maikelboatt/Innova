-- GetRoomTypeAllotmentCalendarQuery.sql - RoomInventory - RoomTypeAllotment

SELECT a.[Date],
       a.TotalRooms,
       a.BookedCount,
       a.TotalRooms - a.BookedCount AS Remaining
FROM roominventory.RoomTypeAllotment a
WHERE a.RoomTypeId = @RoomTypeId
  AND a.[Date] >= @From
  AND a.[Date] < @To
ORDER BY a.[Date];