-- 0005_CreateRoomTypeAllotmentSchema.sql
CREATE TABLE roominventory.RoomTypeAllotment
(
    Id          UNIQUEIDENTIFIER PRIMARY KEY,
    RoomTypeId  UNIQUEIDENTIFIER NOT NULL,
    [Date]      DATE             NOT NULL,
    TotalRooms  INT              NOT NULL,
    BookedCount INT              NOT NULL DEFAULT 0,
    CONSTRAINT FK_RoomTypeAllotment_RoomTypeDefinition FOREIGN KEY (RoomTypeId)
        REFERENCES roominventory.RoomTypeDefinition (Id),
    CONSTRAINT CK_RoomTypeAllotment_BookedCount
        CHECK (BookedCount >= 0 AND BookedCount <= TotalRooms)
);
GO

-- One allotment row per room type per night — GetByRoomTypeAndDateAsync
-- relies on this resolving to exactly one row.
CREATE UNIQUE INDEX UX_RoomTypeAllotment_RoomTypeId_Date
    ON roominventory.RoomTypeAllotment (RoomTypeId, [Date]);
GO