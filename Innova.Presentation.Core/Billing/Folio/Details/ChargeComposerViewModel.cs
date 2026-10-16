namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>Inline form for posting a charge (room service, minibar, laundry...).</summary>
    public sealed class ChargeComposerViewModel:LedgerComposerViewModel
    {
        private string _category = string.Empty;
        private string _description = string.Empty;

        /// <summary>One-tap categories. Charge.Category is free text, so typing something else is fine.</summary>
        public IReadOnlyList<string> CategorySuggestions { get; } =
        [
            "Room Charge", "Restaurant", "Minibar", "Laundry", "Spa", "Transport", "Other"
        ];

        public string Category
        {
            get => _category;
            set
            {
                if (SetProperty(ref _category, value))
                    NotifyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public override bool IsValid => HasValidAmount && !string.IsNullOrWhiteSpace(_category);

        public override void Reset()
        {
            _category = string.Empty;
            _description = string.Empty;
            RaisePropertyChanged(nameof(Category));
            RaisePropertyChanged(nameof(Description));
            SetAmountText(string.Empty);
        }
    }
}
