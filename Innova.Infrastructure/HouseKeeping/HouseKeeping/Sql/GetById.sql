-- GetById.sql - HouseKeeping

SELECT h.Id,
       h.RoomId,
       h.Type,
       h.Status,
       h.AssignedStaffId,
       h.CreatedAt,
       h.CompletedAt,
       h.InspectionPassed,
       h.InspectionNotes
FROM housekeeping.HouseKeepingTask h
WHERE h.Id = @Id;