using Innova.Domain.Billing.Events;
using Innova.Domain.Billing.ValueObjects;
using Innova.Domain.Common;
using Innova.Domain.Shared.Exceptions;
using Innova.Domain.Shared.ValueObjects;

namespace Innova.Domain.Billing.Aggregates
{
    public sealed class Folio:AggregateRoot<FolioId>
    {
        private readonly List<Adjustment> _adjustments = [];
        private readonly List<Charge> _charges = [];
        private readonly List<Payment> _payments = [];
        private readonly List<Refund> _refunds = [];


        private Folio()
        {

        }

        private Folio( FolioId folioId,
                       FolioOwner owner,
                       FolioStatus status,
                       string currency = "GHS" ):base(folioId)
        {
            Owner = owner;
            Currency = currency;
            Status = status;
        }


        public FolioOwner Owner { get; }
        public string Currency { get; } = "GHS";
        public FolioStatus Status { get; private set; }

        public Money Balance => Total(_charges.Select(c => c.Amount))
                                .Subtract(Total(_adjustments.Select(a => a.Amount)))
                                .Subtract(Total(_payments.Select(p => p.Amount)))
                                .Add(Total(_refunds.Select(r => r.Amount)));

        public IReadOnlyCollection<Charge> Charges => _charges.AsReadOnly();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
        public IReadOnlyCollection<Adjustment> Adjustments => _adjustments.AsReadOnly();
        public IReadOnlyCollection<Refund> Refunds => _refunds.AsReadOnly();

        public Money Refundable
        {
            get
            {
                decimal credit = Math.Max(0m, -Balance.Amount);
                decimal netReceived = Total(_payments.Select(p => p.Amount))
                                          .Amount - Total(_refunds.Select(r => r.Amount))
                                          .Amount;

                return Money.Of(Math.Max(0m, Math.Min(credit, netReceived)), Currency);
            }
        }

        private Money Total( IEnumerable<Money> amounts ) => amounts.Aggregate(Money.Zero(Currency), ( sum, next ) => sum.Add(next));

        public static Folio Open( FolioOwner owner, string currency )
        {
            FolioId folioId = FolioId.New();
            Folio folio = new(
                folioId,
                owner,
                FolioStatus.Open,
                currency);

            folio.RaiseDomainEvent(
                new FolioOpened(
                    folioId.Value,
                    owner.OwnerId,
                    owner.Type.ToString(),
                    currency));

            return folio;
        }

        public static Folio Reconstitute( FolioId folioId,
                                          FolioOwner owner,
                                          string currency,
                                          FolioStatus status,
                                          IEnumerable<Charge> charges,
                                          IEnumerable<Payment> payments,
                                          IEnumerable<Adjustment> adjustments,
                                          IEnumerable<Refund> refunds )
        {
            Folio folio = new(
                folioId,
                owner,
                status,
                currency);

            folio._charges.AddRange(charges);
            folio._payments.AddRange(payments);
            folio._adjustments.AddRange(adjustments);
            folio._refunds.AddRange(refunds);

            return folio;
        }

        public void PostCharge( Charge charge )
        {
            EnsureOpen();

            if (charge.Amount.Currency != Currency)
                throw new DomainException("Charge currency does not match this folio's currency");

            _charges.Add(charge);
        }

        public void RecordPayment( Payment payment )
        {
            EnsureOpen();

            if (payment.Amount.Currency != Currency)
                throw new DomainException("Payment currency does not match this folio's currency");

            _payments.Add(payment);
        }

        public void Settle()
        {
            EnsureOpen();
            if (Balance.Amount != 0)
                throw new DomainException($"Cannot settle folio with outstanding balance f {Balance.Amount} {Currency}");


            Status = FolioStatus.Settled;

            RaiseDomainEvent(
                new FolioSettled(
                    Id.Value,
                    Owner.OwnerId,
                    Owner.Type.ToString(),
                    Currency,
                    DateTime.UtcNow));
        }

        public void PostAdjustment( Adjustment adjustment )
        {
            EnsureOpen();

            if (adjustment.Amount.Currency != Currency)
                throw new DomainException("Adjustment currency does not match this folio's currency");

            _adjustments.Add(adjustment);

            RaiseDomainEvent(
                new FolioAdjustmentPosted(
                    Id.Value,
                    Owner.OwnerId,
                    Owner.Type.ToString(),
                    adjustment.Amount.Amount,
                    adjustment.Amount.Currency,
                    adjustment.Type.ToString(),
                    adjustment.Reason,
                    adjustment.PostedAt));
        }

        public void IssueRefund( Refund refund )
        {
            EnsureOpen();

            if (refund.Amount.Currency != Currency)
                throw new DomainException("Refund currency does not match this folio's currency.");

            Money refundable = Refundable;

            if (refund.Amount.Amount > refundable.Amount)
                throw new DomainException(
                    $"Only {refundable.Amount:N2} {Currency} can be refunded on this folio: "
                    + "the lesser of its credit balance and the cash actually received.");

            _refunds.Add(refund);

            RaiseDomainEvent(
                new FolioRefundIssued(
                    Id.Value,
                    Owner.OwnerId,
                    Owner.Type.ToString(),
                    refund.Amount.Amount,
                    refund.Amount.Currency,
                    refund.Method.ToString(),
                    refund.Reference,
                    refund.Reason,
                    refund.RefundedAt));
        }

        public void Void()
        {
            if (Status == FolioStatus.Settled)
                throw new DomainException("Cannot void a folio that has already been settled.");

            Status = FolioStatus.Void;

            RaiseDomainEvent(
                new FolioMarkedVoid(
                    Id.Value,
                    Owner.OwnerId,
                    Owner.Type.ToString(),
                    Currency,
                    DateTime.UtcNow));
        }

        private void EnsureOpen()
        {
            if (Status != FolioStatus.Open)
                throw new DomainException($"Folio is {Status}; charges and payments require an Open folio.");
        }
    }
}
