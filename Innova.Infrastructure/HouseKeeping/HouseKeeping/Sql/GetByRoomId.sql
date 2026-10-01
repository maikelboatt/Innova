-- GetByRoomId.sql - HouseKeeping

SELECT TOP (1) h.Id,
               h.RoomId,
               h.Type,
               h.Status,
               h.AssignedStaffId,
               h.CreatedAt,
               h.CompletedAt,
               h.InspectionPassed,
               h.InspectionNotes
FROM housekeeping.HouseKeepingTask h
WHERE h.RoomId = @RoomId
ORDER BY h.CreatedAt DESC;