using Innova.Domain.Common;

namespace Innova.Domain.RoomInventory.ValueObjects
{
    public sealed class Floor:ValueObject
    {
        private Floor( int level, string? wing )
        {
            Level = level;
            Wing = wing;
        }

        public int Level { get; }
        public string? Wing { get; }

        public static Floor Of( int level, string? wing = null ) =>
            // Negative levels allowed deliberately — basements/parking levels
            // are legitimate in many properties.
            new(level, wing);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Level;
            yield return Wing;
        }
    }
}
