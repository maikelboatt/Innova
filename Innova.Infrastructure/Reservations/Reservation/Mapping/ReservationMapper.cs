using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Infrastructure.Reservations.Reservation.Mapping
{
    public static class ReservationMapper
    {
        public static ReservationRow ToPersistenceModel( Domain.Reservations.Aggregates.Reservation reservation ) => new()
            {
                Id = reservation.Id.Value,
                GuestId = reservation.GuestId.Value,
                GroupBookingId = reservation.GroupBookingId?.Value,
                RoomTypeRequestedId = reservation.RoomTypeRequested.Value,

                StayStart = reservation.StayPeriod.Start,
                StayEnd = reservation.StayPeriod.End,

                NightlyRateAmount =
                    reservation.RatePlan.NightlyRate.Amount,

                NightlyRateCurrency =
                    reservation.RatePlan.NightlyRate.Currency,

                FreeCancellationWindowHrs =
                    reservation.RatePlan.CancellationPolicy
                               .FreeCancellationWindowHours,

                CancellationFeeAmount =
                    reservation.RatePlan.CancellationPolicy.FeeIfWithinWindow.Amount,

                CancellationFeeCurrency =
                    reservation.RatePlan.CancellationPolicy.FeeIfWithinWindow.Currency,

                Status = reservation.Status.ToString()
            };

        public static Domain.Reservations.Aggregates.Reservation ToDomain(
            ReservationRow row )
        {
            Money nightlyRate = Money.Of(row.NightlyRateAmount, row.NightlyRateCurrency);

            Money cancellationFee = Money.Of(row.CancellationFeeAmount, row.CancellationFeeCurrency);

            CancellationPolicy cancellationPolicy = CancellationPolicy.Of(row.FreeCancellationWindowHrs, cancellationFee);

            RatePlan ratePlan = RatePlan.Of(nightlyRate, cancellationPolicy);

            return Domain.Reservations.Aggregates.Reservation.Reconstitute(
                ReservationId.From(row.Id),
                GuestId.From(row.GuestId),
                row.GroupBookingId.HasValue
                    ? GroupBookingId.From(row.GroupBookingId.Value)
                    : null,
                RoomTypeId.From(row.RoomTypeRequestedId),
                DateRange.Of(row.StayStart, row.StayEnd),
                ratePlan,
                MapStringToStatus(row.Status));
        }

        private static ReservationStatus MapStringToStatus( string status )
        {
            return status switch
                   {
                       "Tentative"  => ReservationStatus.Tentative,
                       "Confirmed"  => ReservationStatus.Confirmed,
                       "CheckedIn"  => ReservationStatus.CheckedIn,
                       "CheckedOut" => ReservationStatus.CheckedOut,
                       "Cancelled"  => ReservationStatus.Cancelled,
                       "NoShow"     => ReservationStatus.NoShow,
                       _            => throw new ArgumentException($"Invalid reservation status {status}")
                   };
        }
    }
}
