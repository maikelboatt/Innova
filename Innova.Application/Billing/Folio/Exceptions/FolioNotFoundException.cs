using Innova.Domain.Billing.ValueObjects;

namespace Innova.Application.Billing.Folio.Exceptions
{
    public sealed class FolioNotFoundException( FolioId folioId ):ApplicationException($"Folio '{folioId}' not found.");
}
