-- 0007_CreateReservationSchema.sql
CREATE TABLE reservations.Reservation
(
    Id                        UNIQUEIDENTIFIER PRIMARY KEY,
    GuestId                   UNIQUEIDENTIFIER NOT NULL,
    GroupBookingId            UNIQUEIDENTIFIER NULL,
    RoomTypeRequestedId       UNIQUEIDENTIFIER NOT NULL,
    StayStart                 DATE             NOT NULL,
    StayEnd                   DATE             NOT NULL,
    NightlyRateAmount         DECIMAL(18, 2)   NOT NULL,
    NightlyRateCurrency       CHAR(3)          NOT NULL,
    FreeCancellationWindowHrs INT              NOT NULL,
    CancellationFeeAmount     DECIMAL(18, 2)   NOT NULL,
    CancellationFeeCurrency   CHAR(3)          NOT NULL,
    Status                    NVARCHAR(20)     NOT NULL,
    CONSTRAINT FK_Reservation_Guest FOREIGN KEY (GuestId)
        REFERENCES guestmgmt.Guest (Id),
    CONSTRAINT FK_Reservation_GroupBooking FOREIGN KEY (GroupBookingId)
        REFERENCES reservations.GroupBooking (Id),
    CONSTRAINT FK_Reservation_RoomTypeDefinition FOREIGN KEY (RoomTypeRequestedId)
        REFERENCES roominventory.RoomTypeDefinition (Id),
    CONSTRAINT CK_Reservation_StayDates CHECK (StayEnd > StayStart),
    CONSTRAINT CK_Reservation_Status
        CHECK (Status IN ('Tentative', 'Confirmed', 'CheckedIn', 'CheckedOut', 'Cancelled', 'NoShow'))
);
GO

CREATE INDEX IX_Reservation_GuestId
    ON reservations.Reservation (GuestId);
GO

-- Backs ListReservationsQuery's ArrivingOn/DepartingOn filters and the
-- front-desk arrivals/departures board.
CREATE INDEX IX_Reservation_StayStart_StayEnd
    ON reservations.Reservation (StayStart, StayEnd);
GO

CREATE INDEX IX_Reservation_GroupBookingId
    ON reservations.Reservation (GroupBookingId)
    WHERE GroupBookingId IS NOT NULL;
GO