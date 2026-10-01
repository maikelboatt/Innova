-- CheckAvailabilityQuery.sql - RoomInventory - RoomTypeAllotment

SELECT CAST(
               CASE
                   WHEN COUNT(*) = DATEDIFF(DAY, @CheckIn, @CheckOut)
                       AND MIN(a.TotalRooms - a.BookedCount) > 0
                       THEN 1
                   ELSE 0
                   END
           AS bit
       )       AS HasAvailability,

       CASE
           WHEN COUNT(*) = DATEDIFF(DAY, @CheckIn, @CheckOut)
               THEN COALESCE(MIN(a.TotalRooms - a.BookedCount), 0)
           ELSE 0
           END AS RoomsRemaining
FROM roominventory.RoomTypeAllotment a
WHERE a.RoomTypeId = @RoomTypeId
  AND a.Date >= @CheckIn
  AND a.Date < @CheckOut;