using Innova.Application.Exceptions;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Exceptions
{
    public sealed class FolioNotFoundException( FolioId folioId ):ApplicationExceptions($"Folio '{folioId}' not found.");
}
