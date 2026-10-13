-- OwnerOptions.sql - Billing
-- Lookup for the Open Folio form: what a folio of @OwnerType can be opened for.
-- Exactly one branch returns rows (the others are filtered out by @OwnerType).
-- OpenFolioCount lets the form warn when an owner already has an open folio.

SELECT g.Id                            AS OwnerId,
       g.FirstName + N' ' + g.LastName AS Label,
       g.PhoneNumber                   AS Detail,
       (SELECT COUNT(*)
        FROM billing.Folio f
        WHERE f.OwnerType = N'Guest'
          AND f.OwnerId = g.Id
          AND f.Status = N'Open')      AS OpenFolioCount
FROM guestmgmt.Guest g
WHERE @OwnerType = N'Guest'
  AND g.IsActive = 1

UNION ALL

SELECT g.Id,
       g.FirstName + N' ' + g.LastName,
       N'Room ' + s.AssignedRoomNumber,
       (SELECT COUNT(*)
        FROM billing.Folio f
        WHERE f.OwnerType = N'GuestWithinStay'
          AND f.OwnerId = g.Id
          AND f.Status = N'Open')
FROM frontdesk.StayOccupant so
         INNER JOIN frontdesk.Stay s ON s.Id = so.StayId AND s.ActualCheckOut IS NULL
         INNER JOIN guestmgmt.Guest g ON g.Id = so.GuestId
WHERE @OwnerType = N'GuestWithinStay'

UNION ALL

SELECT s.Id,
       N'Room ' + s.AssignedRoomNumber,
       g.FirstName + N' ' + g.LastName,
       (SELECT COUNT(*)
        FROM billing.Folio f
        WHERE f.OwnerType = N'Stay'
          AND f.OwnerId = s.Id
          AND f.Status = N'Open')
FROM frontdesk.Stay s
         INNER JOIN reservations.Reservation r ON r.Id = s.ReservationId
         INNER JOIN guestmgmt.Guest g ON g.Id = r.GuestId
WHERE @OwnerType = N'Stay'
  AND s.ActualCheckOut IS NULL

UNION ALL

SELECT gb.Id,
       gb.GroupName,
       org.FirstName + N' ' + org.LastName,
       (SELECT COUNT(*)
        FROM billing.Folio f
        WHERE f.OwnerType = N'GroupBooking'
          AND f.OwnerId = gb.Id
          AND f.Status = N'Open')
FROM reservations.GroupBooking gb
         INNER JOIN guestmgmt.Guest org ON org.Id = gb.OrganizerGuestId
WHERE @OwnerType = N'GroupBooking'

ORDER BY 2;