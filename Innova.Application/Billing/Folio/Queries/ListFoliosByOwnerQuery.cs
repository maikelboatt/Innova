using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.DTO;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Queries
{
    public sealed record ListFoliosByOwnerQuery( FolioOwnerType OwnerType, Guid OwnerId ):IQuery<IReadOnlyCollection<FolioSummaryDto>>;
}
