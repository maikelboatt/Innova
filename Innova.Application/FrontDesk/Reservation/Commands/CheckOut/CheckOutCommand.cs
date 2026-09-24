using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.FrontDesk.Reservation.Commands.CheckOut
{
    public sealed record CheckOutCommand( Guid StayId ):ICommand<Guid>;
}
