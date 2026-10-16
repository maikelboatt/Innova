// MediCore.Presentation.Core/Identity/LoginViewModel.cs
//
// Deliberately not built on any existing base class — it doesn't fit
// WorkspaceViewModel (no Shell content-area presence), FormViewModel
// (no unsaved-changes-on-cancel concept for a login screen), or
// DetailsViewModel (nothing being loaded by key). Same reasoning
// ShellViewModel itself extends ViewModelBase directly, not a
// specialized base.

using Innova.Application.Dispatchers;
using Innova.Application.Identity.Commands.AuthenticateUser;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using Innova.Presentation.Core.Shell;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Innova.Presentation.Core.Identity
{
    public sealed class LoginViewModel:ViewModelBase
    {
        private readonly IMvxNavigationService _navigationService;
        private string? _errorMessage;
        private string _password = string.Empty;
        private string _username = string.Empty;

        public LoginViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            IMvxNavigationService navigationService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _navigationService = navigationService;
            LoginCommand = new MvxAsyncCommand(OnLoginAsync, () => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password));
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public IMvxAsyncCommand LoginCommand { get; }

        public string? ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        private async Task OnLoginAsync()
        {
            ErrorMessage = null;
            await RaisePropertyChanged(nameof(HasError));

            Guid? userId = await ExecuteCommandAsync<AuthenticateUserCommand, Guid>(new AuthenticateUserCommand(Username, Password));

            if (userId is not { } id || id == Guid.Empty)
            {
                ErrorMessage = "Invalid username or password.";
                await RaisePropertyChanged(nameof(HasError));
                return;
            }

            await _navigationService.Navigate<ShellViewModel>();
        }
    }
}
