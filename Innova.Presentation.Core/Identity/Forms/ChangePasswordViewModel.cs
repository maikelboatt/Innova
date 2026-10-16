using Innova.Application.Abstractions.Messaging;
using Innova.Application.Dispatchers;
using Innova.Application.Identity.Commands.ChangePassword;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.Identity.Forms
{
    public sealed class ChangePasswordViewModel:FormViewModel
    {
        private readonly IModalService _modalService;
        private string _confirmNewPassword = string.Empty;

        private string _currentPassword = string.Empty;
        private string _newPassword = string.Empty;


        public ChangePasswordViewModel( CommandDispatcher commandDispatcher,
                                        IBusyIndicatorService busyIndicator,
                                        INotificationService notifications,
                                        IMessageBoxService messageBox,
                                        IModalService modalService ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            messageBox)
        {
            _modalService = modalService;
            IsSaveEnabled = false;
        }

        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                if (SetProperty(ref _currentPassword, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set
            {
                if (SetProperty(ref _confirmNewPassword, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        private void RecalculateSaveEnabled() => IsSaveEnabled = !string.IsNullOrWhiteSpace(CurrentPassword) &&
                                                                 NewPassword.Length >= 8 &&
                                                                 ConfirmNewPassword.Length >= 8 &&
                                                                 NewPassword == ConfirmNewPassword;

        protected override async Task OnSaveAsync()
        {
            await ExecuteCommandAsync<ChangePasswordCommand, Unit>(new ChangePasswordCommand(CurrentPassword, NewPassword));

            HasUnsavedChanges = false;
            _modalService.Close();

            Notifications.ShowSuccess("Password changed successfully.");
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();
            return Task.CompletedTask;
        }
    }
}
