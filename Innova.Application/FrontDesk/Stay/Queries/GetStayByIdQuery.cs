using Innova.Application.Abstractions.Messaging;
using Innova.Application.FrontDesk.Stay.DTO;

namespace Innova.Application.FrontDesk.Stay.Queries
{
    public sealed record GetStayByIdQuery( Guid StayId ):IQuery<StayDto?>;
}
