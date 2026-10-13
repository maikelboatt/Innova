using Innova.Application.FrontDesk.Stay.DTO;

namespace Innova.Presentation.Core.FrontDesk.Stay.Lists
{
    public sealed class StayListItemViewModel
    {
        public StayListItemViewModel( StaySummaryDto dto )
        {
            StayId = dto.StayId;
            RoomNumber = dto.RoomNumber;
            ActualCheckIn = dto.ActualCheckIn;

            ActualCheckInDisplay =
                dto.ActualCheckIn.ToString("dd MMM yyyy, hh:mm tt");
        }

        public Guid StayId { get; }

        public string RoomNumber { get; }

        public DateTime ActualCheckIn { get; }

        public string ActualCheckInDisplay { get; }
    }
}
