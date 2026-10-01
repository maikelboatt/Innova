-- Insert.sql - Room Inventory

INSERT INTO roominventory.RoomTypeAllotment
(Id,
 RoomTypeId,
 [Date],
 TotalRooms,
 BookedCount)
VALUES (@Id,
        @RoomTypeId,
        @Date,
        @TotalRooms,
        @BookedCount);