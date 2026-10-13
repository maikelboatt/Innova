namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping
{
    public class StaffOption
    {
        public StaffOption( Guid staffId, string displayName )
        {
            StaffId = staffId;
            DisplayName = displayName;
        }

        public Guid StaffId { get; init; }
        public string DisplayName { get; init; }

        public void Deconstruct( out Guid staffId, out string displayName )
        {
            staffId = StaffId;
            displayName = DisplayName;
        }

        public override string ToString() => DisplayName;
    }
}
