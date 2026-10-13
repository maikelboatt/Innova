-- Update.sql - HouseKeeping

UPDATE housekeeping.HouseKeepingTask
SET Status           = @Status,
    AssignedStaffId  = @AssignedStaffId,
    CompletedAt      = SYSUTCDATETIME(),
    InspectionPassed = @InspectionPassed,
    InspectionNotes  = @InspectionNotes
WHERE Id = @Id;