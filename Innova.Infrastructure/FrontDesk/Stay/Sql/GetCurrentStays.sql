-- GetCurrentStays.sql - FrontDesk

SELECT s.Id,
       s.AssignedRoomNumber,
       s.ActualCheckIn
FROM frontdesk.Stay s
WHERE s.ActualCheckIn IS NOT NULL
  AND s.ActualCheckOut IS NULL
  AND s.ActualCheckIn <= @CurrentDate
ORDER BY s.ActualCheckIn;