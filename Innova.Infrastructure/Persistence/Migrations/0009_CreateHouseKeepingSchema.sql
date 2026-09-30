-- 0009_CreateHouseKeepingSchema.sql
CREATE SCHEMA housekeeping;
GO

CREATE TABLE housekeeping.HouseKeepingTask
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY,
    RoomId           UNIQUEIDENTIFIER NOT NULL, -- No FK: deliberately decoupled from RoomInventory
    Type             NVARCHAR(20)     NOT NULL,
    Status           NVARCHAR(20)     NOT NULL,
    AssignedStaffId  UNIQUEIDENTIFIER NULL,
    CreatedAt        DATETIME2        NOT NULL,
    CompletedAt      DATETIME2        NULL,
    InspectionPassed BIT              NULL,
    InspectionNotes  NVARCHAR(1000)   NULL,
    CONSTRAINT CK_HouseKeepingTask_Type
        CHECK (Type IN ('Cleaning', 'Inspection', 'Maintenance')),
    CONSTRAINT CK_HouseKeepingTask_Status
        CHECK (Status IN ('Pending', 'InProgress', 'Completed', 'Failed'))
);
GO

-- Backs EnsureNoActiveTaskAsync's "does this room already have a Pending or
-- InProgress task" check, plus GetByRoomIdAsync generally.
CREATE INDEX IX_HouseKeepingTask_RoomId_Status
    ON housekeeping.HouseKeepingTask (RoomId, Status);
GO