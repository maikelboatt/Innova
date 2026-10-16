using System.Globalization;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>
    ///     Inline form for handing money back to the guest. The amount is limited to what the folio can refund
    ///     (Folio.Refundable: the lesser of its credit and the cash actually received) and defaults to all of it.
    /// </summary>
    public sealed class RefundComposerViewModel:LedgerComposerViewModel
    {
        private string _currency = string.Empty;
        private string _reason = string.Empty;
        private string _reference = string.Empty;
        private decimal _refundable;

        public RefundComposerViewModel()
        {
            Methods =
            [
                new ComposerChoiceViewModel<PaymentMethod>("Cash", PaymentMethod.Cash),
                new ComposerChoiceViewModel<PaymentMethod>("Card", PaymentMethod.Card),
                new ComposerChoiceViewModel<PaymentMethod>("Bank transfer", PaymentMethod.BankTransfer),
                new ComposerChoiceViewModel<PaymentMethod>("Mobile money", PaymentMethod.MobileMoney)
            ];

            SelectMethod(Methods[0]);
        }

        public IReadOnlyList<ComposerChoiceViewModel<PaymentMethod>> Methods { get; }

        public ComposerChoiceViewModel<PaymentMethod>? SelectedMethod { get; private set; }

        /// <summary>Receipt, POS or transfer number of the money going out. Optional.</summary>
        public string Reference
        {
            get => _reference;
            set => SetProperty(ref _reference, value);
        }

        /// <summary>Required by the domain: a refund always says why.</summary>
        public string Reason
        {
            get => _reason;
            set
            {
                if (SetProperty(ref _reason, value))
                    NotifyChanged();
            }
        }

        public override bool IsValid => HasValidAmount && SelectedMethod is not null && !string.IsNullOrWhiteSpace(_reason);

        public void SelectMethod( ComposerChoiceViewModel<PaymentMethod>? choice )
        {
            foreach (ComposerChoiceViewModel<PaymentMethod> method in Methods)
                method.IsSelected = ReferenceEquals(method, choice);

            SelectedMethod = choice;
            RaisePropertyChanged(nameof(SelectedMethod));
            NotifyChanged();
        }

        /// <summary>Called after every reload with the currency and what can be refunded right now.</summary>
        public void SetContext( string currency, decimal refundable )
        {
            _currency = currency;
            _refundable = refundable;

            // Pre-fill only an untouched box, so a half-typed amount is never overwritten by a reload.
            if (string.IsNullOrWhiteSpace(AmountText) && refundable > 0)
                SetAmountText(refundable.ToString("0.00", CultureInfo.CurrentCulture));
            else
                NotifyChanged();
        }

        protected override string? ValidateAmount( decimal amount ) => amount > _refundable
                                                                           ? $"At most {_currency} {_refundable:N2} can be refunded."
                                                                           : null;

        public override void Reset()
        {
            _reference = string.Empty;
            _reason = string.Empty;
            RaisePropertyChanged(nameof(Reference));
            RaisePropertyChanged(nameof(Reason));
            SelectMethod(Methods[0]);
            SetAmountText(string.Empty);
        }
    }
}
