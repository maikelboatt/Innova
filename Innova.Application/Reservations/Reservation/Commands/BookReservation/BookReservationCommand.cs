using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.Reservation.Commands.BookReservation
{
    public sealed record BookReservationCommand(
        Guid GuestId,
        Guid RoomTypeRequested,
        DateOnly CheckIn,
        DateOnly CheckOut,
        decimal NightlyRateAmount,
        string NightlyRateCurrency,
        int FreeCancellationWindowHours,
        decimal CancellationFeeAmount,
        string CancellationFeeCurrency,
        Guid? GroupBookingId ):ICommand<Guid>;
}
