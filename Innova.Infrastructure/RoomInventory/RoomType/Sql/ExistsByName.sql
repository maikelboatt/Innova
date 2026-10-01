-- ExistsByName.sql - Room Inventory

SELECT CASE
           WHEN EXISTS
               (SELECT 1
                FROM roominventory.RoomTypeDefinition
                WHERE Name = @Name)
               THEN CAST(1 AS BIT)
           ELSE CAST(0 AS BIT)
           END;