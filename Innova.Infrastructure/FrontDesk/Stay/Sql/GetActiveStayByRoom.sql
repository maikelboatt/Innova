-- GetActiveStayByRoom.sql - FrontDesk

SELECT s.Id,
       s.ReservationId,
       s.AssignedRoomId,
       s.AssignedRoomNumber,
       s.GroupBookingId,
       s.MaxOccupancy,
       s.ActualCheckIn,
       s.ActualCheckOut
FROM frontdesk.Stay s
WHERE s.AssignedRoomId = @RoomId
  AND s.ActualCheckOut IS NULL;