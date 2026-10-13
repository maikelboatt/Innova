using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.DTO;

namespace Innova.Application.Billing.Folio.Queries
{
    /// <summary>All folios, open ones first then most recent activity. Status: "Open", "Settled", "Void", or null for all.</summary>
    public sealed record ListFoliosQuery( string? Status = null ):IQuery<IReadOnlyCollection<FolioListItemDto>>;
}
