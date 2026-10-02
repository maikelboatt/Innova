-- Update.sql - Reservation

UPDATE reservations.Reservation
SET GroupBookingId = @GroupBookingId,
    Status         = @Status
WHERE Id = @Id;