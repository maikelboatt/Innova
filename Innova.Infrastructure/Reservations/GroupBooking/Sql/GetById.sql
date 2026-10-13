-- GetById.sql - Reservations
SELECT gb.Id,
       gb.OrganizerGuestId,
       gb.GroupName,
       g.FirstName + ' ' + g.LastName AS OrganizerName
FROM reservations.GroupBooking gb
         INNER JOIN guestmgmt.Guest g
                    ON gb.OrganizerGuestId = g.Id
WHERE gb.Id = @GroupBookingId;

SELECT r.GroupBookingId,
       r.Id AS ReservationId
FROM reservations.Reservation r
WHERE r.GroupBookingId = @GroupBookingId
ORDER BY r.Id;