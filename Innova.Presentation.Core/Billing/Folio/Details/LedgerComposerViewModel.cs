using System.Globalization;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>
    ///     Shared behaviour of the three inline entry forms on the folio details screen (charge, payment, adjustment):
    ///     an amount typed as text and validated to a positive value with at most two decimal places.
    /// </summary>
    public abstract class LedgerComposerViewModel:MvxNotifyPropertyChanged
    {
        private string _amountText = string.Empty;

        public string AmountText
        {
            get => _amountText;
            set
            {
                if (SetProperty(ref _amountText, value))
                    NotifyChanged();
            }
        }

        /// <summary>The parsed amount, or null when nothing (or something invalid) has been typed.</summary>
        public decimal? Amount => AmountError is null && decimal.TryParse(
                                      _amountText.Trim(),
                                      NumberStyles.Number,
                                      CultureInfo.CurrentCulture,
                                      out decimal amount)
                                      ? amount
                                      : null;

        /// <summary>Null while the box is empty (no nagging before the user types) or the text is a valid amount.</summary>
        public string? AmountError
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_amountText)) return null;

                if (!decimal.TryParse(
                        _amountText.Trim(),
                        NumberStyles.Number,
                        CultureInfo.CurrentCulture,
                        out decimal amount))
                    return "Enter a valid amount.";

                if (amount <= 0) return "The amount must be more than zero.";
                if (decimal.Round(amount, 2) != amount) return "Use at most 2 decimal places.";

                return ValidateAmount(amount);
            }
        }

        public bool HasAmountError => AmountError is not null;

        protected bool HasValidAmount => Amount is > 0;

        public abstract bool IsValid { get; }

        /// <summary>Raised after any input changes, so the screen can re-evaluate its Submit button.</summary>
        public event EventHandler? Changed;

        /// <summary>A composer can add its own limit (for example "at most what can be refunded"). Null means fine.</summary>
        protected virtual string? ValidateAmount( decimal amount ) => null;

        public abstract void Reset();

        protected void SetAmountText( string text )
        {
            _amountText = text;
            RaisePropertyChanged(nameof(AmountText));
            NotifyChanged();
        }

        protected void NotifyChanged()
        {
            RaisePropertyChanged(nameof(Amount));
            RaisePropertyChanged(nameof(AmountError));
            RaisePropertyChanged(nameof(HasAmountError));
            RaisePropertyChanged(nameof(IsValid));
            OnDerivedChanged();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Lets a derived composer raise its own computed properties.</summary>
        protected virtual void OnDerivedChanged() { }
    }
}
