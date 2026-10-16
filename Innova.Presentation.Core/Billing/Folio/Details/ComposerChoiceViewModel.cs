using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Billing.Folio.Details
{
    /// <summary>A selectable chip in a composer, e.g. "Card" (payment method) or "Goodwill credit" (adjustment type).</summary>
    public sealed class ComposerChoiceViewModel<T>( string label, T value ):MvxNotifyPropertyChanged
    {
        private bool _isSelected;

        public string Label { get; } = label;
        public T Value { get; } = value;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
