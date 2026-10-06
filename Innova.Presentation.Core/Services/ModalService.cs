using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Services
{
    public sealed class ModalService( IMvxIoCProvider iocProvider ):IModalService
    {
        private MvxViewModel? _currentModalViewModel;
        private TaskCompletionSource<bool>? _tcs;

        public MvxViewModel? CurrentModalViewModel
        {
            get => _currentModalViewModel;
            private set
            {
                _currentModalViewModel = value;
                ModalChanged?.Invoke();
            }
        }

        public event Action? ModalChanged;

        public async Task ShowAsync<TViewModel>() where TViewModel : MvxViewModel
        {
            TViewModel vm = iocProvider.Resolve<TViewModel>();
            await vm.Initialize();
            _tcs = new TaskCompletionSource<bool>();
            CurrentModalViewModel = vm;
            await _tcs.Task;
        }

        public async Task ShowAsync<TViewModel, TParam>( TParam parameter )
            where TViewModel : MvxViewModel
        {
            TViewModel vm = iocProvider.Resolve<TViewModel>();

            // If ViewModel accepts a typed parameter, call Prepare
            if (vm is IMvxViewModel<TParam> paramVm)
                paramVm.Prepare(parameter);

            await vm.Initialize();
            _tcs = new TaskCompletionSource<bool>();
            CurrentModalViewModel = vm;
            await _tcs.Task;
        }

        // ══════════════════════════════════════════════════════════════
        // NavigateAsync — swaps CurrentModalViewModel in place. Deliberately
        // does NOT touch _tcs: the caller that originally awaited
        // ShowAsync (e.g. PatientListViewModel awaiting the Details modal)
        // stays suspended across this swap, and only resumes when Close()
        // is eventually called — whether that's from Details, from Edit,
        // or from anywhere else in the session.
        // ══════════════════════════════════════════════════════════════
        public async Task NavigateAsync<TViewModel>() where TViewModel : MvxViewModel
        {
            if (_tcs is null)
                throw new InvalidOperationException(
                    "NavigateAsync requires an open modal session. Call ShowAsync first.");

            TViewModel vm = iocProvider.Resolve<TViewModel>();
            await vm.Initialize();
            CurrentModalViewModel = vm;
        }

        public async Task NavigateAsync<TViewModel, TParam>( TParam parameter )
            where TViewModel : MvxViewModel
        {
            if (_tcs is null)
                throw new InvalidOperationException(
                    "NavigateAsync requires an open modal session. Call ShowAsync first.");

            TViewModel vm = iocProvider.Resolve<TViewModel>();

            if (vm is IMvxViewModel<TParam> paramVm)
                paramVm.Prepare(parameter);

            await vm.Initialize();
            CurrentModalViewModel = vm;
        }

        public void Close()
        {
            CurrentModalViewModel = null;
            _tcs?.TrySetResult(true);
            _tcs = null;
        }
    }
}
