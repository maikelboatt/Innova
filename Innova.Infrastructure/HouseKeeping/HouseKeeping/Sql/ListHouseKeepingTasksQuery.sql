-- ListHouseKeepingTasksQuery.sql - HouseKeeping

SELECT h.Id,
       h.RoomId,
       h.Type,
       h.Status,
       h.AssignedStaffId
FROM housekeeping.HouseKeepingTask h
WHERE (@Status IS NULL OR h.Status = @Status)
  AND (@AssignedStaffId IS NULL OR h.AssignedStaffId = @AssignedStaffId)
  AND (@RoomId IS NULL OR h.RoomId = @RoomId)
ORDER BY h.CreatedAt DESC;