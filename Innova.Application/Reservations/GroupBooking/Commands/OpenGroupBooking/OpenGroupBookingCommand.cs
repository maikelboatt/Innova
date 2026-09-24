using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Reservations.GroupBooking.Commands.OpenGroupBooking
{
    public sealed record OpenGroupBookingCommand( Guid OrganizerGuestId, string GroupName ):ICommand<Guid>;
}
