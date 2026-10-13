-- GetAll.sql Reservations

SELECT gb.Id,
       gb.OrganizerGuestId,
       gb.GroupName,
       g.FirstName + ' ' + g.LastName AS OrganizerName
FROM reservations.GroupBooking gb
         INNER JOIN guestmgmt.Guest g
                    ON gb.OrganizerGuestId = g.Id
ORDER BY gb.GroupName;

SELECT r.GroupBookingId,
       r.Id AS ReservationId
FROM reservations.Reservation r
WHERE r.GroupBookingId IS NOT NULL
ORDER BY r.GroupBookingId, r.Id;