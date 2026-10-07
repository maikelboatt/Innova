using Innova.Application.Exceptions;
using Innova.Domain.FrontDesk.ValueObjects;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Application.Billing.Folio.Exceptions
{
    public sealed class OutstandingBalanceException( StayId stayId, Money outstandingBalance )
        :ApplicationExceptions($"An outstanding balance of '{outstandingBalance.Currency} {outstandingBalance.Amount}' exist for Stay '{stayId}'");
}
