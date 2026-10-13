using Innova.Application.Billing.Folio.DTO;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Billing.Folio.Forms
{
    /// <summary>One selectable row in the owner picker (a stay, a guest or a group). Display model only.</summary>
    public sealed class FolioOwnerOptionViewModel:MvxNotifyPropertyChanged
    {
        private bool _isSelected;

        public FolioOwnerOptionViewModel( FolioOwnerOptionDto dto )
        {
            OwnerId = dto.OwnerId;
            Label = dto.Label;
            Detail = dto.Detail ?? string.Empty;
            OpenFolioCount = dto.OpenFolioCount;
            OpenFoliosDisplay = dto.OpenFolioCount == 1
                                    ? "1 open folio"
                                    : $"{dto.OpenFolioCount} open folios";
        }

        public Guid OwnerId { get; }
        public string Label { get; }
        public string Detail { get; }
        public int OpenFolioCount { get; }
        public bool HasOpenFolios => OpenFolioCount > 0;
        public string OpenFoliosDisplay { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
}
