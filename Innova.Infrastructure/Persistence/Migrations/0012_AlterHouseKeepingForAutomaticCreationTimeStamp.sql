-- Add a default constraint to automatically set CreatedAt to UTC
ALTER TABLE housekeeping.HouseKeepingTask
    ADD CONSTRAINT DF_HouseKeepingTask_CreatedAt
        DEFAULT SYSUTCDATETIME() FOR CreatedAt;
GO