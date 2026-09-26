using Innova.Application.Abstractions.Messaging;
using Innova.Application.GuestManagement.Guest.DTO;

namespace Innova.Application.GuestManagement.Guest.Queries
{
    public sealed record GetGuestByIdQuery( Guid GuestId ):IQuery<GuestDto?>;
}
