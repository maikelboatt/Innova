using Innova.Application.Abstractions.Messaging;
using Innova.Application.GuestManagement.Guest.DTO;

namespace Innova.Application.GuestManagement.Guest.Queries
{
    public record GetAllGuestQuery:IQuery<IReadOnlyCollection<GuestDto>>;
}
