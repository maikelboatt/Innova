using Innova.Application.Dispatchers;
using Innova.Application.Identity.Commands.RegisterUser;
using Innova.Domain.Identity.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.Identity.Forms
{
    public sealed class RegisterUserViewModel:FormViewModel
    {
        private readonly IModalService _modalService;
        private string _password = string.Empty;
        private Role? _selectedRole;

        private string _username = string.Empty;

        public RegisterUserViewModel( CommandDispatcher commandDispatcher,
                                      IBusyIndicatorService busyIndicator,
                                      INotificationService notifications,
                                      IMessageBoxService messageBox,
                                      IModalService modalService )
            :base(
                commandDispatcher,
                busyIndicator,
                notifications,
                messageBox)
        {
            _modalService = modalService;
            IsSaveEnabled = false;
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public Role? SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public IReadOnlyList<Role> Roles { get; } = [Role.Admin, Role.Receptionist, Role.Manager, Role.HousekeepingStaff, Role.BillingStaff];

        private void RecalculateSaveEnabled() => IsSaveEnabled = !string.IsNullOrWhiteSpace(Username) && Password.Length >= 8 && SelectedRole is not null;

        protected override async Task OnSaveAsync()
        {
            Guid? userId = await ExecuteCommandAsync<RegisterUserCommand, Guid>(
                               new RegisterUserCommand(Username, Password, SelectedRole!.Value));

            if (userId is not { } id || id == Guid.Empty)
                return;

            HasUnsavedChanges = false;
            _modalService.Close();
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();
            return Task.CompletedTask;
        }
    }
}
