using Innova.Application.Abstractions.Messaging;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Commands.OpenFolio
{
    public sealed record OpenFolioCommand( FolioOwnerType OwnerType, Guid OwnerId, string Currency ):ICommand<Guid>;
}
