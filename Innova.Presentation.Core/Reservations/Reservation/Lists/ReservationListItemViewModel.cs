using Innova.Application.Reservations.DTO;

namespace Innova.Presentation.Core.Reservations.Reservation.Lists
{
    public sealed class ReservationListItemViewModel
    {
        public ReservationListItemViewModel( ReservationDto dto )
        {
            ReservationId = dto.ReservationId;
            GuestId = dto.GuestId;
            GroupBookingId = dto.GroupBookingId;
            RoomTypeRequested = dto.RoomTypeRequested;
            RoomTypeName = dto.RoomTypeName;
            GuestName = dto.GuestName;

            CheckIn = dto.CheckIn;
            CheckOut = dto.CheckOut;

            CheckInDisplay = dto.CheckIn.ToString("dd MMM yyyy");
            CheckOutDisplay = dto.CheckOut.ToString("dd MMM yyyy");

            NightlyRateAmount = dto.NightlyRateAmount;
            NightlyRateCurrency = dto.NightlyRateCurrency;
            NightlyRateDisplay =
                $"{dto.NightlyRateCurrency} {dto.NightlyRateAmount:N2}";

            Status = dto.Status;

            (StatusLabel, StatusBadge) = MapStatus(dto.Status);

            BookingType = dto.GroupBookingId.HasValue
                              ? "Group"
                              : "Individual";
        }

        public Guid ReservationId { get; }
        public Guid GuestId { get; }
        public Guid? GroupBookingId { get; }

        public Guid RoomTypeRequested { get; }
        public string RoomTypeName { get; }
        public string GuestName { get; }

        public DateOnly CheckIn { get; }
        public DateOnly CheckOut { get; }

        public string CheckInDisplay { get; }
        public string CheckOutDisplay { get; }

        public decimal NightlyRateAmount { get; }
        public string NightlyRateCurrency { get; }
        public string NightlyRateDisplay { get; }

        public string Status { get; }
        public string StatusLabel { get; }
        public string StatusBadge { get; }

        public string BookingType { get; }

        public string StayPeriodDisplay => $"{CheckInDisplay} → {CheckOutDisplay}";

        private static (string Label, string Badge) MapStatus( string status ) => status switch
                                                                                  {
                                                                                      "Tentative"  => ("Tentative", "Warning"),
                                                                                      "Confirmed"  => ("Confirmed", "Success"),
                                                                                      "CheckedIn"  => ("Checked In", "Info"),
                                                                                      "CheckedOut" => ("Checked Out", "Neutral"),
                                                                                      "Cancelled"  => ("Cancelled", "Danger"),
                                                                                      "NoShow"     => ("No-Show", "Danger"),
                                                                                      _            => (status, "Neutral")
                                                                                  };
    }
}
