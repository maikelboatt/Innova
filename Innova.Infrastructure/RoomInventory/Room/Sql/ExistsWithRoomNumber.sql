-- ExistsWithRoomNumber.sql - Room Inventory

SELECT CASE
           WHEN EXISTS
               (SELECT 1
                FROM roominventory.Room
                WHERE RoomNumber = @RoomNumber)
               THEN CAST(1 AS BIT)
           ELSE CAST(0 AS BIT)
           END;