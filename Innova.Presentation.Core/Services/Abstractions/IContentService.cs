using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Services.Abstractions
{
    public interface IContentService
    {
        MvxViewModel CurrentViewModel { get; }
        event EventHandler<MvxViewModel>? CurrentViewModelChanged;

        void NavigateTo<TViewModel>() where TViewModel : MvxViewModel;

        void NavigateTo<TViewModel>( object parameter ) where TViewModel : MvxViewModel;
    }
}
