using Innova.Application.RoomInventory.DTO;

namespace Innova.Presentation.Core.RoomInventory.RoomTypeAllotment.Lists
{
    /// <summary>
    ///     One cell of the calendar: a single room type on a single night.
    ///     A night with no allotment row is still a cell (HasAllotment = false) so the
    ///     columns of every row stay aligned with the day headers.
    /// </summary>
    public sealed class AllotmentCellViewModel
    {
        // Utilisation at or above this is shown as "filling up".
        private const int WarningThresholdPercent = 80;

        private AllotmentCellViewModel( Guid roomTypeId,
                                        string roomTypeName,
                                        DateOnly date,
                                        bool hasAllotment,
                                        int totalRooms,
                                        int bookedCount,
                                        int remaining )
        {
            RoomTypeId = roomTypeId;
            RoomTypeName = roomTypeName;
            Date = date;
            HasAllotment = hasAllotment;
            TotalRooms = totalRooms;
            BookedCount = bookedCount;
            Remaining = remaining;

            IsClosed = hasAllotment && totalRooms == 0;
            ShowTotal = hasAllotment && totalRooms > 0;
            UtilizationPercentage = totalRooms > 0
                                        ? (int)Math.Round(bookedCount * 100.0 / totalRooms)
                                        : 0;

            // Badge is driven by utilisation, not by an absolute "rooms left" number:
            // a 2-room type with nothing booked must not look like a warning.
            Badge = !hasAllotment || IsClosed ? "None"
                    : remaining <= 0 ? "Danger"
                    : UtilizationPercentage >= WarningThresholdPercent ? "Warning"
                    : "Success";

            RemainingDisplay = !hasAllotment ? "–"
                               : IsClosed ? "Closed"
                               : remaining <= 0 ? "Full"
                               : remaining.ToString();

            TotalDisplay = ShowTotal
                               ? $"of {totalRooms}"
                               : string.Empty;

            ToolTipText = BuildToolTip();
        }

        public Guid RoomTypeId { get; }
        public string RoomTypeName { get; }
        public DateOnly Date { get; }

        public bool HasAllotment { get; }
        public bool IsClosed { get; }
        public bool ShowTotal { get; }

        public int TotalRooms { get; }
        public int BookedCount { get; }
        public int Remaining { get; }
        public int UtilizationPercentage { get; }

        /// <summary>"None", "Success", "Warning" or "Danger" - the view maps these to status brushes.</summary>
        public string Badge { get; }

        public string RemainingDisplay { get; }
        public string TotalDisplay { get; }
        public string ToolTipText { get; }

        public static AllotmentCellViewModel ForNight( Guid roomTypeId, string roomTypeName, AllotmentNightDto dto ) => new(
            roomTypeId,
            roomTypeName,
            dto.Date,
            true,
            dto.TotalRooms,
            dto.BookedCount,
            dto.Remaining);

        public static AllotmentCellViewModel NotConfigured( Guid roomTypeId, string roomTypeName, DateOnly date ) => new(
            roomTypeId,
            roomTypeName,
            date,
            false,
            0,
            0,
            0);

        private string BuildToolTip()
        {
            string heading = $"{RoomTypeName} · {Date:ddd dd MMM yyyy}";

            if (!HasAllotment)
                return $"{heading}\nNo allotment set up for this night";

            if (IsClosed)
                return $"{heading}\nClosed for sale (0 rooms allotted)";

            return $"{heading}\n{Remaining} of {TotalRooms} rooms available ({UtilizationPercentage}% booked)";
        }
    }
}
