using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Billing.Folio
{
    public sealed record FolioSettlementResult( Money TotalSettled, IReadOnlyCollection<Guid> SettledFolioIds );
}
