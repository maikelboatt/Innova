-- List.sql - Billing
-- One row per folio. The owner is resolved to a display name per OwnerType; the totals and the balance come from
-- billing.FolioBalance so this query never carries its own copy of the balance formula.
-- @Status and @FolioId are both optional filters (NULL = no filter). @FolioId is used by the folio details
-- screen to fetch a single folio's header with the same owner resolution as the list.

SELECT f.Id                                                                                                                                          AS FolioId,
       f.OwnerType,
       f.OwnerId,
       COALESCE(CASE f.OwnerType
                    WHEN N'Guest' THEN ownerGuest.FirstName + N' ' + ownerGuest.LastName
                    WHEN N'GuestWithinStay' THEN ownerGuest.FirstName + N' ' + ownerGuest.LastName
                    WHEN N'Stay' THEN N'Room ' + ownerStay.AssignedRoomNumber
                    WHEN N'GroupBooking' THEN ownerGroup.GroupName
                    END,
                N'(unknown)')                                                                                                                        AS OwnerName,
       CASE f.OwnerType
           WHEN N'Guest' THEN ownerGuest.PhoneNumber
           WHEN N'GuestWithinStay' THEN N'Room ' + guestStay.AssignedRoomNumber
           WHEN N'Stay' THEN booker.FirstName + N' ' + booker.LastName
           WHEN N'GroupBooking' THEN organizer.FirstName + N' ' + organizer.LastName
           END                                                                                                                                       AS OwnerDetail,
       f.Currency,
       f.Status,
       b.TotalCharges,
       b.TotalPayments,
       b.TotalRefunds,
       b.TotalAdjustments,
       b.Balance,
       (SELECT MAX(v.Moment)
        FROM (VALUES ((SELECT MAX(c2.PostedAt)
                       FROM billing.Charge c2
                       WHERE c2.FolioId = f.Id)), ((SELECT MAX(p2.ReceivedAt)
                                                    FROM billing.Payment p2
                                                    WHERE p2.FolioId = f.Id)), ((SELECT MAX(r2.RefundedAt)
                                                                                 FROM billing.Refund r2
                                                                                 WHERE r2.FolioId = f.Id)), ((SELECT MAX(a2.PostedAt)
                                                                                                              FROM billing.Adjustment a2
                                                                                                              WHERE a2.FolioId = f.Id))) v (Moment)) AS LastActivityAt
FROM billing.Folio f
         INNER JOIN billing.FolioBalance b ON b.FolioId = f.Id
         LEFT JOIN guestmgmt.Guest ownerGuest
                   ON ownerGuest.Id = f.OwnerId AND f.OwnerType IN (N'Guest', N'GuestWithinStay')
         LEFT JOIN frontdesk.Stay ownerStay
                   ON ownerStay.Id = f.OwnerId AND f.OwnerType = N'Stay'
         LEFT JOIN reservations.Reservation stayReservation
                   ON stayReservation.Id = ownerStay.ReservationId
         LEFT JOIN guestmgmt.Guest booker
                   ON booker.Id = stayReservation.GuestId
         LEFT JOIN reservations.GroupBooking ownerGroup
                   ON ownerGroup.Id = f.OwnerId AND f.OwnerType = N'GroupBooking'
         LEFT JOIN guestmgmt.Guest organizer
                   ON organizer.Id = ownerGroup.OrganizerGuestId
         OUTER APPLY (SELECT TOP (1) st.AssignedRoomNumber
                      FROM frontdesk.StayOccupant so
                               INNER JOIN frontdesk.Stay st ON st.Id = so.StayId
                      WHERE f.OwnerType = N'GuestWithinStay'
                        AND so.GuestId = f.OwnerId
                      ORDER BY st.ActualCheckIn DESC) guestStay
WHERE (@Status IS NULL OR f.Status = @Status)
  AND (@FolioId IS NULL OR f.Id = @FolioId)
ORDER BY CASE f.Status WHEN N'Open' THEN 0 WHEN N'Settled' THEN 1 ELSE 2 END,
         LastActivityAt DESC,
         f.Id;