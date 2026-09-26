using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.FrontDesk.Stay.Commands.CheckIn
{
    public sealed record CheckInCommand( Guid ReservationId, Guid PrimaryOccupantId ):ICommand<Guid>;
}
