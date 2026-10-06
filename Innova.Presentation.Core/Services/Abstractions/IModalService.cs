// MediCore.Presentation.Core/Services/Abstractions/IModalService.cs

using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Services.Abstractions
{
    public interface IModalService
    {
        MvxViewModel? CurrentModalViewModel { get; }
        event Action? ModalChanged;

        // Starts a new modal session. Resolves only when Close() is
        // called — i.e. when the whole session (which may pass through
        // several NavigateAsync swaps) ends.
        Task ShowAsync<TViewModel>() where TViewModel : MvxViewModel;

        Task ShowAsync<TViewModel, TParam>( TParam parameter ) where TViewModel : MvxViewModel;

        // Swaps the View shown inside an already-open session, without
        // resolving the ShowAsync caller's await. Throws if no session
        // is currently open — NavigateAsync is not a substitute for
        // ShowAsync, it only makes sense mid-session.
        Task NavigateAsync<TViewModel>() where TViewModel : MvxViewModel;

        Task NavigateAsync<TViewModel, TParam>( TParam parameter ) where TViewModel : MvxViewModel;

        void Close();
    }
}
