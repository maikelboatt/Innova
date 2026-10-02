-- List.sql Reservation

SELECT r.Id,
       r.GuestId,
       r.StayStart,
       r.StayEnd,
       r.Status
FROM reservations.Reservation r
WHERE (@GuestId IS NULL OR r.GuestId = @GuestId)
  AND (@Status IS NULL OR r.Status = @Status)
  AND (@ArrivingOn IS NULL OR r.StayStart = @ArrivingOn)
  AND (@DepartingOn IS NULL OR r.StayEnd = @DepartingOn)
ORDER BY r.StayStart, r.Id;