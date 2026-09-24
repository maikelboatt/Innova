using Innova.Application.Abstractions.Services;
using Innova.Application.FrontDesk.Reservation.Exceptions;
using Innova.Domain.FrontDesk.Aggregates;
using Innova.Domain.FrontDesk.Repositories;
using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Domain.HouseKeeping.ValueObjects;
using Innova.Domain.Reservations.Aggregates;
using Innova.Domain.Reservations.ValueObjects;
using Innova.Domain.RoomInventory.Aggregates;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Services
{
    public sealed class StayService(
        IStayRepository stayRepository,
        IReservationService reservationService,
        IRoomService roomService,
        IRoomTypeService roomTypeService,
        IHouseKeepingService houseKeepingService,
        IBillingAssemblyService billingAssemblyService
    ):IStayService
    {
        public async Task<Stay> CheckInAsync( ReservationId reservationId, GuestId primaryOccupant, CancellationToken ct = default )
        {
            // 1. Read the reservation — need to know which room type was
            //    actually booked before anything else can happen.
            Reservation reservation = await reservationService.GetByIdAsync(reservationId, ct);

            // 2. Find a physically vacant room of that type.
            Room room = await roomService.FindVacantRoomAsync(reservation.RoomTypeRequested, ct)
                        ?? throw new NoVacantRoomException(reservation.RoomTypeRequested);

            // 3. RoomTypeDefinition supplies the MaxOccupancy snapshot Stay
            //    needs at creation — read before Occupy() so a failure here
            //    doesn't leave the room wrongly marked Occupied.
            RoomTypeDefinition roomType = await roomTypeService.GetByIdAsync(reservation.RoomTypeRequested, ct);

            // 4. Only now does anything actually mutate: the room flips to
            //    Occupied.
            await roomService.OccupyAsync(room.Id, ct);

            // 5. Create and persist the Stay itself.
            Stay stay = Stay.CheckIn(
                reservationId,
                room.Id,
                room.Number,
                roomType.MaxOccupancy,
                primaryOccupant,
                reservation.GroupBookingId);

            await stayRepository.SaveAsync(stay, ct);

            // 6. Close the loop on the Reservation side.
            await reservationService.MarkCheckedInAsync(reservationId, ct);

            return stay;
        }

        public async Task<Stay> CheckOutAsync( StayId stayId, CancellationToken ct = default )
        {

            Stay stay = await GetByIdAsync(stayId, ct);

            // NOTE: currency is hardcoded here rather than resolved from
            // anywhere — fine for a single-currency, single-property
            // deployment, which is the only thing this domain currently
            // models. A multi-currency setup would need this resolved from
            // hotel configuration instead, not assumed.
            Money outstandingBalance = await billingAssemblyService.GetTotalOutstandingBalanceAsync(
                                           stay.Id.Value,
                                           stay
                                               .Occupants.Select(g => g.Value)
                                               .ToList(),
                                           stay.GroupBookingId?.Value,
                                           "GHS",
                                           ct);

            if (outstandingBalance.Amount != 0)

                throw new OutstandingBalanceException(stayId, outstandingBalance);

            stay.CheckOut();

            await stayRepository.UpdateAsync(stay, ct);

            await roomService.MarkDirtyAsync(stay.AssignedRoomId, ct);

            // Closes the loop Housekeeping was built for — without this,
            // nothing ever creates a cleaning task; a dirty room just sits
            // there until someone notices and schedules one manually.
            await houseKeepingService.ScheduleAsync(stay.AssignedRoomId.Value, HouseKeepingTaskType.Cleaning, ct);

            await reservationService.MarkCheckedOutAsync(stay.ReservationId, ct);

            return stay;
        }

        public async Task<Stay> AddOccupantAsync( StayId stayId, GuestId guestId, CancellationToken ct = default )
        {
            Stay stay = await GetByIdAsync(stayId, ct);

            stay.AddOccupant(guestId);

            await stayRepository.UpdateAsync(stay, ct);

            return stay;
        }

        public async Task<Stay> GetByIdAsync( StayId stayId, CancellationToken ct = default ) =>
            await stayRepository.GetByIdAsync(stayId, ct) ?? throw new StayNotFoundException(stayId);
    }
}
