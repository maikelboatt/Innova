using Innova.Domain.RoomInventory.ValueObjects;

namespace Innova.Infrastructure.RoomInventory.Room.Mapping
{
    public static class RoomMapper
    {
        public static RoomRow ToPersistenceModel( Domain.RoomInventory.Aggregates.Room room )
        {
            Guid roomId = room.Id.Value;
            RoomRow row = new()
                          {
                              Id = roomId,
                              RoomNumber = room.Number.Value,
                              FloorLevel = room.Floor.Level,
                              FloorWing = room.Floor.Wing,
                              RoomTypeId = room.RoomTypeId.Value,
                              Status = room.Status.ToString()
                          };

            return row;
        }

        public static IReadOnlyCollection<Domain.RoomInventory.Aggregates.Room> ToDomain(
            IReadOnlyCollection<RoomRow> rows ) => rows
                                                   .Select(ToDomain)
                                                   .ToList()
                                                   .AsReadOnly();

        public static Domain.RoomInventory.Aggregates.Room ToDomain( RoomRow row ) => Domain.RoomInventory.Aggregates.Room.Reconstitute(
            RoomId.From(row.Id),
            RoomNumber.Of(row.RoomNumber),
            Floor.Of(row.FloorLevel, row.FloorWing),
            RoomTypeId.From(row.RoomTypeId),
            MapStringToStatus(row.Status));


        private static RoomStatus MapStringToStatus( string status )
        {
            return status switch
                   {
                       "Vacant"       => RoomStatus.Vacant,
                       "Occupied"     => RoomStatus.Occupied,
                       "OutOfService" => RoomStatus.OutOfService,
                       "Dirty"        => RoomStatus.Dirty,
                       _              => throw new ArgumentException($"Invalid room status {status}")
                   };
        }
    }
}
