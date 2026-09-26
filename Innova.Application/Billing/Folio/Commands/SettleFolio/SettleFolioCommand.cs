using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Billing.Folio.Commands.SettleFolio
{
    public sealed record SettleFolioCommand( Guid FolioId ):ICommand<Unit>;
}
