-- ListRoomTypesQuery.sql - Room Inventory - RoomType

SELECT rt.Id,
       rt.Name,
       rt.MaxOccupancy,
       rt.BaseRateAmount,
       rt.BaseRateCurrency
FROM roominventory.RoomTypeDefinition rt
ORDER BY rt.Name;