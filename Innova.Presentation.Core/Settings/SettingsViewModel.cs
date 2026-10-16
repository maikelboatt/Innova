using System.Collections.ObjectModel;
using Innova.Application.Abstractions.Services;
using Innova.Application.Dispatchers;
using Innova.Application.Identity.Commands.DeactivateUser;
using Innova.Application.Identity.Commands.ReactivateUser;
using Innova.Application.Identity.DTO;
using Innova.Application.Identity.Queries.GetAllUsers;
using Innova.Application.Shared.DTO;
using Innova.Application.Shared.Queries.GetRecentAuditLog;
using Innova.Domain.Identity.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Identity;
using Innova.Presentation.Core.Identity.Forms;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Innova.Presentation.Core.Settings
{
    public sealed class SettingsViewModel:WorkspaceViewModel
    {
        private readonly ICurrentUserContext _currentUser;
        private readonly IMessageBoxService _messageBox;
        private readonly IModalService _modalService;
        private readonly IMvxNavigationService _navigationService;
        private readonly QueryDispatcher _queryDispatcher;

        private string _activeSection = "Account";

        public SettingsViewModel(
            CommandDispatcher commandDispatcher,
            QueryDispatcher queryDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            IModalService modalService,
            IMessageBoxService messageBox,
            ICurrentUserContext currentUser,
            IMvxNavigationService navigationService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;
            _messageBox = messageBox;
            _currentUser = currentUser;
            _navigationService = navigationService;

            Title = "Settings";

            SelectSectionCommand = new MvxCommand<string>(OnSelectSection);
            SignOutCommand = new MvxAsyncCommand(OnSignOutAsync);
            ChangePasswordCommand = new MvxAsyncCommand(OnChangePasswordAsync);
            OpenRegisterUserCommand = new MvxAsyncCommand(OnOpenRegisterUserAsync);
            DeactivateUserCommand = new MvxAsyncCommand<UserRowViewModel>(OnDeactivateUserAsync);
            ReactivateUserCommand = new MvxAsyncCommand<UserRowViewModel>(OnReactivateUserAsync);
        }

        protected override async Task LoadAsync()
        {
            // User administration and audit information are restricted
            // to administrators.
            if (!IsAdmin)
                return;

            await LoadUsersAsync();
            await LoadAuditLogAsync();
        }

        private async Task LoadUsersAsync()
        {
            IEnumerable<UserSummaryDto> users = await _queryDispatcher.DispatchAsync(new GetAllUsersQuery());

            Users.Clear();

            foreach (UserSummaryDto dto in users)
            {
                Users.Add(new UserRowViewModel(dto, CurrentUserId));
            }
        }

        private async Task LoadAuditLogAsync()
        {
            IEnumerable<AuditLogDto> entries = await _queryDispatcher.DispatchAsync(new GetRecentAuditLogQuery());

            AuditLogEntries.Clear();

            foreach (AuditLogDto dto in entries)
            {
                AuditLogEntries.Add(new AuditLogRowViewModel(dto));
            }
        }

#region Account

        public string CurrentUsername => _currentUser.Username ?? string.Empty;

        public string CurrentUserRole => _currentUser.Role?.Value ?? string.Empty;

        public Guid CurrentUserId => _currentUser.UserId ?? Guid.Empty;

        public bool IsAdmin => _currentUser.Role == Role.Admin;

#endregion

#region Navigation

        public string ActiveSection
        {
            get => _activeSection;
            private set
            {
                if (SetProperty(ref _activeSection, value))
                {
                    RaisePropertyChanged(nameof(IsAccountActive));
                    RaisePropertyChanged(nameof(IsUsersActive));
                    RaisePropertyChanged(nameof(IsAuditLogActive));
                }
            }
        }

        public bool IsAccountActive => ActiveSection == "Account";

        public bool IsUsersActive => ActiveSection == "Users";

        public bool IsAuditLogActive => ActiveSection == "AuditLog";

        public IMvxCommand<string> SelectSectionCommand { get; }

        private void OnSelectSection( string section )
        {
            if (string.IsNullOrWhiteSpace(section))
                return;

            // Keep the role restriction in the UI as well as
            // enforcing it in the application authorization pipeline.
            if (!IsAdmin && section is "Users" or "AuditLog")
                return;

            ActiveSection = section;
        }

#endregion

#region Collections

        public ObservableCollection<UserRowViewModel> Users { get; }
            = [];

        public ObservableCollection<AuditLogRowViewModel> AuditLogEntries
        {
            get;
        } = [];

#endregion

#region Commands

        public IMvxAsyncCommand SignOutCommand { get; }
        public IMvxAsyncCommand ChangePasswordCommand { get; }
        public IMvxAsyncCommand OpenRegisterUserCommand { get; }
        public IMvxAsyncCommand<UserRowViewModel> DeactivateUserCommand { get; }
        public IMvxAsyncCommand<UserRowViewModel> ReactivateUserCommand { get; }

#endregion

#region Account Actions

        private async Task OnSignOutAsync()
        {
            _currentUser.SignOut();

            await _navigationService.Navigate<LoginViewModel>();
        }

        private async Task OnChangePasswordAsync()
        {
            await _modalService.ShowAsync<ChangePasswordViewModel>();
        }

#endregion

#region User Administration

        private async Task OnOpenRegisterUserAsync()
        {
            if (!IsAdmin)
                return;

            await _modalService.ShowAsync<RegisterUserViewModel>();

            await LoadUsersAsync();
        }

        private async Task OnDeactivateUserAsync( UserRowViewModel? user )
        {
            if (!IsAdmin || user is null)
                return;

            // UX protection only. The application command handler
            // must independently enforce this restriction.
            if (user.UserId == CurrentUserId)
            {
                Notifications.ShowWarning(
                    "You cannot deactivate your own account.");

                return;
            }

            bool confirmed = await _messageBox.ShowConfirmAsync(
                                 "Deactivate User",
                                 $"Deactivate {user.Username}? " +
                                 "They will be unable to sign in until their account " +
                                 "is reactivated.");

            if (!confirmed)
                return;

            await ExecuteCommandAsync(
                new DeactivateUserCommand(user.UserId));

            await LoadUsersAsync();
        }

        private async Task OnReactivateUserAsync( UserRowViewModel? user )
        {
            if (!IsAdmin || user is null)
                return;

            await ExecuteCommandAsync(
                new ReactivateUserCommand(user.UserId));

            await LoadUsersAsync();
        }

#endregion
    }
}
