-- Insert.sql - Reservation

INSERT INTO reservations.Reservation
(Id,
 GuestId,
 GroupBookingId,
 RoomTypeRequestedId,
 StayStart,
 StayEnd,
 NightlyRateAmount,
 NightlyRateCurrency,
 FreeCancellationWindowHrs,
 CancellationFeeAmount,
 CancellationFeeCurrency,
 Status)
VALUES (@Id,
        @GuestId,
        @GroupBookingId,
        @RoomTypeRequestedId,
        @StayStart,
        @StayEnd,
        @NightlyRateAmount,
        @NightlyRateCurrency,
        @FreeCancellationWindowHrs,
        @CancellationFeeAmount,
        @CancellationFeeCurrency,
        @Status);