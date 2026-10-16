using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.DTO;

namespace Innova.Application.Billing.Folio.Queries
{
    /// <summary>
    ///     One folio's header: owner name, status and totals (the same row shape as the folio list).
    ///     GetFolioByIdQuery returns the entries but not the owner's name; the details screen needs both.
    ///     Null when the folio does not exist.
    /// </summary>
    public sealed record GetFolioSummaryQuery( Guid FolioId ):IQuery<FolioListItemDto?>;
}
