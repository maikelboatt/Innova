-- GetById.sql - Reservations
SELECT gb.Id,
       gb.OrganizerGuestId,
       gb.GroupName
FROM reservations.GroupBooking gb
WHERE gb.Id = @GroupBookingId;

SELECT r.GroupBookingId,
       r.Id AS ReservationId
FROM reservations.Reservation r
WHERE r.GroupBookingId = @GroupBookingId
ORDER BY r.Id;