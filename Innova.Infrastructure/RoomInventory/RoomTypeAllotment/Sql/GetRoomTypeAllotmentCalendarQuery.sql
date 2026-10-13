-- GetRoomTypeAllotmentCalendarQuery.sql - RoomInventory - RoomTypeAllotment

SELECT a.[Date],
       a.TotalRooms,
       a.BookedCount,
       a.TotalRooms - a.BookedCount AS Remaining,
       a.Id                         AS RoomTypeId,
       rtd.Name                     AS RoomTypeName
FROM roominventory.RoomTypeAllotment a
         INNER JOIN roominventory.RoomTypeDefinition rtd
                    ON a.RoomTypeId = rtd.Id
WHERE a.RoomTypeId = @RoomTypeId
  AND a.[Date] >= @From
  AND a.[Date] < @To
ORDER BY a.[Date];