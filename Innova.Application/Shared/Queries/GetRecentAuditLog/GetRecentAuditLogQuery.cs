using Innova.Application.Abstractions.Messaging;
using Innova.Application.Shared.DTO;

namespace Innova.Application.Shared.Queries.GetRecentAuditLog
{
    public sealed record GetRecentAuditLogQuery( int Take = 100 ):IQuery<IEnumerable<AuditLogDto>>;
}
