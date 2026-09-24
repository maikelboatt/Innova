using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Services;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Reservations.Reservation.Commands.BookReservation
{
    public sealed class BookReservationCommandHandler( IReservationService reservationService, IDomainEventDispatcher eventDispatcher )
        :ICommandHandler<BookReservationCommand, Guid>
    {
        public async Task<Guid> HandleAsync( BookReservationCommand command, CancellationToken ct = default )
        {
            GuestId guestId = GuestId.From(command.GuestId);
            RoomTypeId roomTypeRequested = RoomTypeId.From(command.RoomTypeRequested);
            DateRange stayPeriod = DateRange.Of(command.CheckIn, command.CheckOut);
            CancellationPolicy cancellationPolicy = CancellationPolicy.Of(
                command.FreeCancellationWindowHours,
                Money.Of(command.CancellationFeeAmount, command.CancellationFeeCurrency));
            RatePlan ratePlan = RatePlan.Of(Money.Of(command.NightlyRateAmount, command.NightlyRateCurrency), cancellationPolicy);
            GroupBookingId groupBookingId = GroupBookingId.From(command.GroupBookingId.Value);

            Domain.Reservations.Aggregates.Reservation reservation = await reservationService.BookAsync(
                                                                         guestId,
                                                                         roomTypeRequested,
                                                                         stayPeriod,
                                                                         ratePlan,
                                                                         groupBookingId,
                                                                         ct);

            await eventDispatcher.DispatchAsync(reservation.DomainEvents, ct);
            reservation.ClearDomainEvents();

            return reservation.Id.Value;
        }
    }
}
