using Innova.Domain.Billing.ValueObjects;

namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>
    ///     Inline form for a credit that reduces the balance. "Refund" is deliberately not offered:
    ///     Folio.Balance subtracts every adjustment, so a refund would move an overpaid folio further from zero.
    /// </summary>
    public sealed class AdjustmentComposerViewModel:LedgerComposerViewModel
    {
        private string _reason = string.Empty;

        public AdjustmentComposerViewModel()
        {
            Types =
            [
                new ComposerChoiceViewModel<AdjustmentType>("Goodwill credit", AdjustmentType.GoodwillCredit),
                new ComposerChoiceViewModel<AdjustmentType>("Billing correction", AdjustmentType.BillingCorrection)
            ];

            SelectType(Types[0]);
        }

        public IReadOnlyList<ComposerChoiceViewModel<AdjustmentType>> Types { get; }

        public ComposerChoiceViewModel<AdjustmentType>? SelectedType { get; private set; }

        public string TypeHint => SelectedType?.Value switch
                                  {
                                      AdjustmentType.GoodwillCredit    => "A discount given to the guest, for example after a complaint.",
                                      AdjustmentType.BillingCorrection => "Removes a charge that was posted by mistake.",
                                      _                                => string.Empty
                                  };

        public string Reason
        {
            get => _reason;
            set
            {
                if (SetProperty(ref _reason, value))
                    NotifyChanged();
            }
        }

        public override bool IsValid => HasValidAmount && SelectedType is not null && !string.IsNullOrWhiteSpace(_reason);

        public void SelectType( ComposerChoiceViewModel<AdjustmentType>? choice )
        {
            foreach (ComposerChoiceViewModel<AdjustmentType> type in Types)
                type.IsSelected = ReferenceEquals(type, choice);

            SelectedType = choice;
            RaisePropertyChanged(nameof(SelectedType));
            NotifyChanged();
        }

        public override void Reset()
        {
            _reason = string.Empty;
            RaisePropertyChanged(nameof(Reason));
            SelectType(Types[0]);
            SetAmountText(string.Empty);
        }

        protected override void OnDerivedChanged() => RaisePropertyChanged(nameof(TypeHint));
    }
}
