using Innova.Application.Reservations.DTO;

namespace Innova.Presentation.Core.Reservations.GroupBooking.Lists
{
    public sealed class GroupBookingListItemViewModel
    {
        public GroupBookingListItemViewModel( GroupBookingDto dto )
        {
            GroupBookingId = dto.GroupBookingId;
            OrganizerGuestId = dto.OrganizerGuestId;
            OrganizerName = dto.OrganizerName;

            GroupName = dto.GroupName;

            ReservationIds = dto.ReservationIds;
            ReservationCount = dto.ReservationIds.Count;

            ReservationCountDisplay =
                dto.ReservationIds.Count == 1
                    ? "1 Reservation"
                    : $"{dto.ReservationIds.Count} Reservations";
        }

        public Guid GroupBookingId { get; }

        public Guid OrganizerGuestId { get; }

        public string OrganizerName { get; }
        public string GroupName { get; }

        public IReadOnlyCollection<Guid> ReservationIds { get; }

        public int ReservationCount { get; }

        public string ReservationCountDisplay { get; }
    }
}
