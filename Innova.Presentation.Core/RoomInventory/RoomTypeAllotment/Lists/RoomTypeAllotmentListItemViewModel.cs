using Innova.Application.RoomInventory.DTO;

namespace Innova.Presentation.Core.RoomInventory.RoomTypeAllotment.Lists
{
    /// <summary>
    ///     One row of the allotment calendar: a room type and its cell for every day
    ///     in the visible window. Display model only.
    /// </summary>
    public sealed class RoomTypeAllotmentListItemViewModel
    {
        public RoomTypeAllotmentListItemViewModel( Guid roomTypeId,
                                                   string roomTypeName,
                                                   IReadOnlyList<AllotmentDayViewModel> days,
                                                   IReadOnlyCollection<AllotmentNightDto> nights )
        {
            RoomTypeId = roomTypeId;
            RoomTypeName = roomTypeName;

            Dictionary<DateOnly, AllotmentNightDto> byDate = nights
                                                             .GroupBy(night => night.Date)
                                                             .ToDictionary(group => group.Key, group => group.First());

            // One cell per visible day, in day order. Nights without an allotment row become
            // "not configured" cells instead of being skipped, which keeps columns aligned.
            Cells =
            [
                .. days.Select(day => byDate.TryGetValue(day.Date, out AllotmentNightDto? night)
                                          ? AllotmentCellViewModel.ForNight(roomTypeId, roomTypeName, night)
                                          : AllotmentCellViewModel.NotConfigured(roomTypeId, roomTypeName, day.Date))
            ];

            int unconfigured = Cells.Count(cell => !cell.HasAllotment);
            HasGaps = unconfigured > 0;
            GapsDisplay = unconfigured switch
                          {
                              0 => string.Empty,
                              1 => "1 night not set up",
                              _ => $"{unconfigured} nights not set up"
                          };
        }

        public Guid RoomTypeId { get; }
        public string RoomTypeName { get; }
        public IReadOnlyList<AllotmentCellViewModel> Cells { get; }

        public bool HasGaps { get; }
        public string GapsDisplay { get; }
    }
}
