using Innova.Application.Abstractions.Messaging;

namespace Innova.Application.Billing.Folio.Commands.VoidFolio
{
    public sealed record VoidFolioCommand( Guid FolioId ):ICommand<Unit>;
}
