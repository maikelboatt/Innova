using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Mapping
{
    public static class RoomTypeAllotmentMapper
    {
        public static AllotmentRow ToPersistenceModel(
            Domain.RoomInventory.Aggregates.RoomTypeAllotment allotment ) => new()
                                                                             {
                                                                                 Id = allotment.Id.Value,
                                                                                 RoomTypeId = allotment.RoomTypeId.Value,
                                                                                 Date = allotment.Date,
                                                                                 TotalRooms = allotment.TotalRooms,
                                                                                 BookedCount = allotment.BookedCount
                                                                             };

        public static IReadOnlyCollection<Domain.RoomInventory.Aggregates.RoomTypeAllotment> ToDomain(
            IEnumerable<AllotmentRow> rows ) => rows
                                                .Select(ToDomain)
                                                .ToList()
                                                .AsReadOnly();

        public static Domain.RoomInventory.Aggregates.RoomTypeAllotment ToDomain( AllotmentRow row ) =>
            Domain.RoomInventory.Aggregates.RoomTypeAllotment.Reconstitute(
                RoomTypeAllotmentId.From(row.Id),
                RoomTypeId.From(row.RoomTypeId),
                row.Date,
                row.TotalRooms,
                row.BookedCount);
    }
}
