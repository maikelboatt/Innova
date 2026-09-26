using Innova.Application.Abstractions.Messaging;
using Innova.Application.Billing.Folio.DTO;

namespace Innova.Application.Billing.Folio.Queries
{
    public sealed record GetFolioByIdQuery( Guid FolioId ):IQuery<FolioDto?>;
}
