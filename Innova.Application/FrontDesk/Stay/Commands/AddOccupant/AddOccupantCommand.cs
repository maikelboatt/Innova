using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.FrontDesk.Stay.Commands.AddOccupant
{
    public sealed record AddOccupantCommand( Guid StayId, Guid GuestId ):ICommand<Guid>;
}
