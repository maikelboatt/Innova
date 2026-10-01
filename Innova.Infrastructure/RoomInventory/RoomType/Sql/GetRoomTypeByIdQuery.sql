-- GetRoomTypeByIdQuery.sql - RoomInventory - RoomType

SELECT rt.Id,
       rt.Name,
       rt.MaxOccupancy,
       rt.BaseRateAmount,
       rt.BaseRateCurrency
FROM roominventory.RoomTypeDefinition rt
WHERE rt.Id = @RoomTypeId;