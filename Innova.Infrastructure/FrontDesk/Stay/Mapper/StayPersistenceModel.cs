namespace Innova.Infrastructure.FrontDesk.Stay.Mapper
{
    public sealed class StayPersistenceModel
    {
        public required StayRow Stay { get; init; }
        public required IReadOnlyCollection<StayOccupantRow> Occupants { get; init; }
    }
}
