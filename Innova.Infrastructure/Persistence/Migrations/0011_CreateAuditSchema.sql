-- 0011_CreateAuditSchema.sql
CREATE SCHEMA audit;
GO

CREATE TABLE audit.AuditLog
(
    Id         INT IDENTITY PRIMARY KEY,
    EventName  NVARCHAR(100)    NOT NULL,
    EntityType NVARCHAR(100)    NOT NULL,
    EntityId   UNIQUEIDENTIFIER NOT NULL,
    Summary    NVARCHAR(1000)   NOT NULL,
    Username   NVARCHAR(100)    NULL,
    OccurredAt DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE INDEX IX_AuditLog_EntityId
    ON audit.AuditLog (EntityId);
GO

CREATE INDEX IX_AuditLog_EventName
    ON audit.AuditLog (EventName);
GO

CREATE INDEX IX_AuditLog_OccurredAt
    ON audit.AuditLog (OccurredAt DESC);
GO