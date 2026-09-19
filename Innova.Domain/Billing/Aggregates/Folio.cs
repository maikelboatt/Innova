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

        public Money Balance => _charges
                                .Aggregate(Money.Zero(Currency), ( acc, c ) => acc.Add(c.Amount))
                                .Subtract(_payments.Aggregate(Money.Zero(Currency), ( acc, p ) => acc.Add(p.Amount)))
                                .Subtract(_adjustments.Aggregate(Money.Zero(Currency), ( acc, a ) => acc.Add(a.Amount)));

        public IReadOnlyCollection<Charge> Charges => _charges.AsReadOnly();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
        public IReadOnlyCollection<Adjustment> Adjustments => _adjustments.AsReadOnly();

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
                                          IEnumerable<Payment> payments )
        {
            Folio folio = new(
                folioId,
                owner,
                status,
                currency);

            folio._charges.AddRange(charges);
            folio._payments.AddRange(payments);

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
