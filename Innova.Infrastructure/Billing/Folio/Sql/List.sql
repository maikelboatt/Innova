-- List.sql - Billing
-- One row per folio. The owner is resolved to a display name per OwnerType, and the totals are computed here
-- so the list does not load charge collections. Balance uses the domain formula: charges - payments - adjustments.

SELECT f.Id                                                  AS FolioId,
       f.OwnerType,
       f.OwnerId,
       COALESCE(CASE f.OwnerType
                    WHEN N'Guest' THEN ownerGuest.FirstName + N' ' + ownerGuest.LastName
                    WHEN N'GuestWithinStay' THEN ownerGuest.FirstName + N' ' + ownerGuest.LastName
                    WHEN N'Stay' THEN N'Room ' + ownerStay.AssignedRoomNumber
                    WHEN N'GroupBooking' THEN ownerGroup.GroupName
                    END, N'(unknown)')                       AS OwnerName,
       CASE f.OwnerType
           WHEN N'Guest' THEN ownerGuest.PhoneNumber
           WHEN N'GuestWithinStay' THEN N'Room ' + guestStay.AssignedRoomNumber
           WHEN N'Stay' THEN booker.FirstName + N' ' + booker.LastName
           WHEN N'GroupBooking' THEN organizer.FirstName + N' ' + organizer.LastName
           END                                               AS OwnerDetail,
       f.Currency,
       f.Status,
       t.TotalCharges,
       t.TotalPayments,
       t.TotalAdjustments,
       t.TotalCharges - t.TotalPayments - t.TotalAdjustments AS Balance,
       t.LastActivityAt
FROM billing.Folio f
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
         CROSS APPLY (SELECT (SELECT COALESCE(SUM(c.Amount), 0)
                              FROM billing.Charge c
                              WHERE c.FolioId = f.Id)                                                                                         AS TotalCharges,
                             (SELECT COALESCE(SUM(p.Amount), 0)
                              FROM billing.Payment p
                              WHERE p.FolioId = f.Id)                                                                                         AS TotalPayments,
                             (SELECT COALESCE(SUM(a.Amount), 0)
                              FROM billing.Adjustment a
                              WHERE a.FolioId = f.Id)                                                                                         AS TotalAdjustments,
                             (SELECT MAX(v.Moment)
                              FROM (VALUES ((SELECT MAX(c2.PostedAt)
                                             FROM billing.Charge c2
                                             WHERE c2.FolioId = f.Id)), ((SELECT MAX(p2.ReceivedAt)
                                                                          FROM billing.Payment p2
                                                                          WHERE p2.FolioId = f.Id)), ((SELECT MAX(a2.PostedAt)
                                                                                                       FROM billing.Adjustment a2
                                                                                                       WHERE a2.FolioId = f.Id))) v (Moment)) AS LastActivityAt) t
WHERE (@Status IS NULL OR f.Status = @Status)
ORDER BY CASE f.Status WHEN N'Open' THEN 0 WHEN N'Settled' THEN 1 ELSE 2 END,
         t.LastActivityAt DESC,
         f.Id;