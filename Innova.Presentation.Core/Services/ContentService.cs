using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Services
{
    public sealed class ContentService( IMvxIoCProvider iocProvider ):IContentService
    {
        private MvxViewModel _currentViewModel = default!;

        public MvxViewModel CurrentViewModel
        {
            get => _currentViewModel;
            private set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke(this, value);
            }
        }

        public event EventHandler<MvxViewModel>? CurrentViewModelChanged;

        public void NavigateTo<TViewModel>() where TViewModel : MvxViewModel
        {
            TViewModel viewModel = iocProvider.Resolve<TViewModel>();
            _ = viewModel?.Initialize();
            CurrentViewModel = viewModel;
        }

        public void NavigateTo<TViewModel>( object parameter ) where TViewModel : MvxViewModel
        {
            TViewModel viewModel = iocProvider.Resolve<TViewModel>();

            // If the ViewModel accepts a parameter, call Prepare
            if (viewModel is IMvxViewModel<object> paramVm)
                paramVm.Prepare(parameter);

            _ = viewModel?.Initialize();
            CurrentViewModel = viewModel;
        }
    }
}
