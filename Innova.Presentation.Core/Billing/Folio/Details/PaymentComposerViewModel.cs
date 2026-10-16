using System.Globalization;
using Innova.Domain.Billing.ValueObjects;

namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>Inline form for recording a payment. The amount is pre-filled with what is owed.</summary>
    public sealed class PaymentComposerViewModel:LedgerComposerViewModel
    {
        private string _currency = string.Empty;
        private decimal _outstanding;
        private string _reference = string.Empty;

        public PaymentComposerViewModel()
        {
            Methods =
            [
                new ComposerChoiceViewModel<PaymentMethod>("Cash", PaymentMethod.Cash),
                new ComposerChoiceViewModel<PaymentMethod>("Card", PaymentMethod.Card),
                new ComposerChoiceViewModel<PaymentMethod>("Bank transfer", PaymentMethod.BankTransfer),
                new ComposerChoiceViewModel<PaymentMethod>("Mobile money", PaymentMethod.MobileMoney)
            ];

            SelectMethod(Methods[0]); // cash is the most common
        }

        public IReadOnlyList<ComposerChoiceViewModel<PaymentMethod>> Methods { get; }

        public ComposerChoiceViewModel<PaymentMethod>? SelectedMethod { get; private set; }

        /// <summary>Receipt, POS or transfer number. Optional in the domain.</summary>
        public string Reference
        {
            get => _reference;
            set => SetProperty(ref _reference, value);
        }

        public string? OverpaymentWarning => Amount is { } amount && amount > Math.Max(_outstanding, 0)
                                                 ? $"That is more than is owed ({_currency} {Math.Max(_outstanding, 0):N2}). The folio will be left in credit."
                                                 : null;

        public bool HasOverpaymentWarning => OverpaymentWarning is not null;

        public override bool IsValid => HasValidAmount && SelectedMethod is not null;

        public void SelectMethod( ComposerChoiceViewModel<PaymentMethod>? choice )
        {
            foreach (ComposerChoiceViewModel<PaymentMethod> method in Methods)
                method.IsSelected = ReferenceEquals(method, choice);

            SelectedMethod = choice;
            RaisePropertyChanged(nameof(SelectedMethod));
            NotifyChanged();
        }

        /// <summary>Called after every reload so the composer knows the currency and what is owed.</summary>
        public void SetContext( string currency, decimal outstanding )
        {
            _currency = currency;
            _outstanding = outstanding;

            // Pre-fill only an untouched box, so a half-typed amount is never overwritten by a reload.
            if (string.IsNullOrWhiteSpace(AmountText) && outstanding > 0)
                SetAmountText(outstanding.ToString("0.00", CultureInfo.CurrentCulture));
            else
                NotifyChanged();
        }

        public override void Reset()
        {
            _reference = string.Empty;
            RaisePropertyChanged(nameof(Reference));
            SelectMethod(Methods[0]);
            SetAmountText(string.Empty);
        }

        protected override void OnDerivedChanged()
        {
            RaisePropertyChanged(nameof(OverpaymentWarning));
            RaisePropertyChanged(nameof(HasOverpaymentWarning));
        }
    }
}
