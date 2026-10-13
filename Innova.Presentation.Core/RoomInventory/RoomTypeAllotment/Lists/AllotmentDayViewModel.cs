namespace Innova.Presentation.Core.RoomInventory.RoomTypeAllotment.Lists
{
    /// <summary>
    ///     One column header of the allotment calendar (a single night).
    ///     Display model only: no commands, no dependencies.
    /// </summary>
    public sealed class AllotmentDayViewModel
    {
        public AllotmentDayViewModel( DateOnly date, DateOnly today, bool showMonth )
        {
            Date = date;
            DayOfWeekLabel = date.ToString("ddd");
            DayNumber = date.Day.ToString();

            // Shown on the first column and on the 1st of each month. When hidden it is an
            // empty string (not collapsed) so every header cell keeps the same height.
            MonthLabel = showMonth
                             ? date.ToString("MMM")
                             : string.Empty;

            IsToday = date == today;
            IsWeekend = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        }

        public DateOnly Date { get; }
        public string DayOfWeekLabel { get; }
        public string DayNumber { get; }
        public string MonthLabel { get; }
        public bool IsToday { get; }
        public bool IsWeekend { get; }
    }
}
