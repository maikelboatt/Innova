-- GetById.sql Reservation

SELECT r.Id,
       r.GuestId,
       r.GroupBookingId,
       r.RoomTypeRequestedId,
       r.StayStart,
       r.StayEnd,
       r.NightlyRateAmount,
       r.NightlyRateCurrency,
       r.FreeCancellationWindowHrs,
       r.CancellationFeeAmount,
       r.CancellationFeeCurrency,
       r.Status,
       rtd.Name                       AS RoomTypeName,
       g.FirstName + ' ' + g.LastName AS GuestName
FROM reservations.Reservation r
         INNER JOIN roominventory.RoomTypeDefinition rtd
                    ON r.RoomTypeRequestedId = rtd.Id
         INNER JOIN guestmgmt.Guest g
                    ON r.GuestId = g.Id
WHERE r.Id = @ReservationId;