using Innova.Domain.Billing.ValueObjects;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Billing.Folio.Forms
{
    /// <summary>A selectable "what is this folio for" card in the Open Folio form. Display model only.</summary>
    public sealed class FolioOwnerTypeOptionViewModel(
        FolioOwnerType type,
        string label,
        string description,
        string ownerNoun ):MvxNotifyPropertyChanged
    {
        private bool _isSelected;

        public FolioOwnerType Type { get; } = type;
        public string Label { get; } = label;
        public string Description { get; } = description;

        /// <summary>Used in prompts such as "Choose the stay".</summary>
        public string OwnerNoun { get; } = ownerNoun;

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
