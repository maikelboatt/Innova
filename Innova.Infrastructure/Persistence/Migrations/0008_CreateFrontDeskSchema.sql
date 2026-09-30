-- 0008_CreateFrontDeskSchema.sql
CREATE SCHEMA frontdesk;
GO

CREATE TABLE frontdesk.Stay
(
    Id                 UNIQUEIDENTIFIER PRIMARY KEY,
    ReservationId      UNIQUEIDENTIFIER NOT NULL,
    GroupBookingId     UNIQUEIDENTIFIER NULL,
    AssignedRoomId     UNIQUEIDENTIFIER NOT NULL,
    AssignedRoomNumber NVARCHAR(20)     NOT NULL,
    MaxOccupancy       INT              NOT NULL,
    ActualCheckIn      DATETIME2        NULL,
    ActualCheckOut     DATETIME2        NULL,
    CONSTRAINT FK_Stay_Reservation FOREIGN KEY (ReservationId)
        REFERENCES reservations.Reservation (Id),
    CONSTRAINT FK_Stay_GroupBooking FOREIGN KEY (GroupBookingId)
        REFERENCES reservations.GroupBooking (Id),
    CONSTRAINT FK_Stay_Room FOREIGN KEY (AssignedRoomId)
        REFERENCES roominventory.Room (Id)
);
GO

-- Stay.Occupants is a genuine many-to-many — several guests per stay, and a
-- guest could occupy different stays over time. Unlike GroupBooking's
-- reservation list, there's no natural FK home on either side, so this
-- needs its own junction table.
CREATE TABLE frontdesk.StayOccupant
(
    StayId  UNIQUEIDENTIFIER NOT NULL,
    GuestId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_StayOccupant PRIMARY KEY (StayId, GuestId),
    CONSTRAINT FK_StayOccupant_Stay FOREIGN KEY (StayId)
        REFERENCES frontdesk.Stay (Id),
    CONSTRAINT FK_StayOccupant_Guest FOREIGN KEY (GuestId)
        REFERENCES guestmgmt.Guest (Id)
);
GO

-- Backs ListCurrentStaysQuery (WHERE ActualCheckOut IS NULL).
CREATE INDEX IX_Stay_ActualCheckOut
    ON frontdesk.Stay (ActualCheckOut);
GO