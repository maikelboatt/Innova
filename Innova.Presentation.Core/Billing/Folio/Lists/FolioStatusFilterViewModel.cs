using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Billing.Folio.Lists
{
    /// <summary>A status chip above the folio list ("Open", "Settled", "Void", "All"). Value null means no filter.</summary>
    public sealed class FolioStatusFilterViewModel( string label, string? status ):MvxNotifyPropertyChanged
    {
        private bool _isActive;

        public string Label { get; } = label;
        public string? Status { get; } = status;

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
    }
}
