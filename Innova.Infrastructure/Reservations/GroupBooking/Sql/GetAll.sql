-- GetAll.sql Reservations

SELECT gb.Id,
       gb.OrganizerGuestId,
       gb.GroupName
FROM reservations.GroupBooking gb
ORDER BY gb.GroupName;

SELECT r.GroupBookingId,
       r.Id AS ReservationId
FROM reservations.Reservation r
WHERE r.GroupBookingId IS NOT NULL
ORDER BY r.GroupBookingId, r.Id;