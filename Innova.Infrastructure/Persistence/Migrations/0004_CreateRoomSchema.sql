-- 0004_CreateRoomSchema.sql
CREATE TABLE roominventory.Room
(
    Id         UNIQUEIDENTIFIER PRIMARY KEY,
    RoomNumber NVARCHAR(20)     NOT NULL,
    FloorLevel INT              NOT NULL,
    FloorWing  NVARCHAR(50)     NULL,
    RoomTypeId UNIQUEIDENTIFIER NOT NULL,
    Status     NVARCHAR(30)     NOT NULL,
    CONSTRAINT FK_Room_RoomTypeDefinition FOREIGN KEY (RoomTypeId)
        REFERENCES roominventory.RoomTypeDefinition (Id),
    CONSTRAINT CK_Room_Status CHECK (Status IN ('Vacant', 'Occupied', 'Dirty', 'OutOfService'))
);
GO

-- Enforces the room-number uniqueness invariant added via
-- IRoomRepository.ExistsWithRoomNumberAsync.
CREATE UNIQUE INDEX UX_Room_RoomNumber
    ON roominventory.Room (RoomNumber);
GO

-- Backs IRoomService.FindVacantRoomAsync(roomTypeId) — filters on exactly
-- this pair.
CREATE INDEX IX_Room_RoomTypeId_Status
    ON roominventory.Room (RoomTypeId, Status);
GO