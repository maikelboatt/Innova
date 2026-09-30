-- 0006_CreateGroupBookingSchema.sql
CREATE SCHEMA reservations;
GO

-- No ReservationIds column here — the relationship lives on
-- Reservation.GroupBookingId (the FK on the "many" side). GroupBooking's
-- in-memory _reservationIds list is reconstituted by querying Reservation
-- WHERE GroupBookingId = @id, not stored redundantly on this table.
CREATE TABLE reservations.GroupBooking
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY,
    OrganizerGuestId UNIQUEIDENTIFIER NOT NULL,
    GroupName        NVARCHAR(200)    NOT NULL,
    CONSTRAINT FK_GroupBooking_Guest FOREIGN KEY (OrganizerGuestId)
        REFERENCES guestmgmt.Guest (Id)
);
GO