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
       r.Status
FROM reservations.Reservation r
WHERE r.Id = @ReservationId;