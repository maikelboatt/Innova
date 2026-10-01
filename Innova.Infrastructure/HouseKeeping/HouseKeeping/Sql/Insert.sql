-- Insert.sql - HouseKeeping

INSERT INTO housekeeping.HouseKeepingTask
(Id,
 RoomId,
 Type,
 Status,
 AssignedStaffId,
 CreatedAt,
 CompletedAt,
 InspectionPassed,
 InspectionNotes)
VALUES (@Id,
        @RoomId,
        @Type,
        @Status,
        @AssignedStaffId,
        @CreatedAt,
        @CompletedAt,
        @InspectionPassed,
        @InspectionNotes);