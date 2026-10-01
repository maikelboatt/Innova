-- Update.sql - HouseKeeping

UPDATE housekeeping.HouseKeepingTask
SET Status           = @Status,
    AssignedStaffId  = @AssignedStaffId,
    CompletedAt      = @CompletedAt,
    InspectionPassed = @InspectionPassed,
    InspectionNotes  = @InspectionNotes
WHERE Id = @Id;