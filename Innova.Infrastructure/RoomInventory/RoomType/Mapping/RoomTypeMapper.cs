using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Infrastructure.RoomInventory.RoomType.Mapping
{
    public static class RoomTypeMapper
    {
        public static RoomTypeRow ToPersistenceModel( RoomTypeDefinition roomType )
        {
            RoomTypeRow row = new()
                              {
                                  Id = roomType.Id.Value,
                                  Name = roomType.Name,
                                  MaxOccupancy = roomType.MaxOccupancy.Value,
                                  BaseRateAmount = roomType.BaseRate.Amount,
                                  BaseRateCurrency = roomType.BaseRate.Currency
                              };

            return row;
        }

        public static IReadOnlyCollection<RoomTypeDefinition> ToDomain(
            IReadOnlyCollection<RoomTypeRow> rows ) => rows
                                                       .Select(ToDomain)
                                                       .ToList()
                                                       .AsReadOnly();

        public static RoomTypeDefinition ToDomain( RoomTypeRow row ) => RoomTypeDefinition.Reconstitute(
            RoomTypeId.From(row.Id),
            row.Name,
            MaxOccupancy.Of(row.MaxOccupancy),
            Money.Of(row.BaseRateAmount, row.BaseRateCurrency));
    }
}
