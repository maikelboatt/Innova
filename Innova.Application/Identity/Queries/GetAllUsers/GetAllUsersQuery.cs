using Innova.Application.Abstractions.Messaging;
using Innova.Application.Identity.DTO;

namespace Innova.Application.Identity.Queries.GetAllUsers
{
    public record GetAllUsersQuery:IQuery<IEnumerable<UserSummaryDto>>;
}
