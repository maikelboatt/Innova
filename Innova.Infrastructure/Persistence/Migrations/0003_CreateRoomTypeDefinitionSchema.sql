-- 0003_CreateRoomTypeDefinitionSchema.sql
CREATE SCHEMA roominventory;
GO

CREATE TABLE roominventory.RoomTypeDefinition
(
    Id               UNIQUEIDENTIFIER PRIMARY KEY,
    Name             NVARCHAR(100)  NOT NULL,
    MaxOccupancy     INT            NOT NULL,
    BaseRateAmount   DECIMAL(18, 2) NOT NULL,
    BaseRateCurrency CHAR(3)        NOT NULL
);
GO

-- Not currently enforced at the application layer (flagged as a minor gap
-- earlier: two room types could both be named "Deluxe King"). Added here
-- proactively since a unique index costs nothing extra — drop it if you'd
-- rather allow duplicate names.
CREATE UNIQUE INDEX UX_RoomTypeDefinition_Name
    ON roominventory.RoomTypeDefinition (Name);
GO