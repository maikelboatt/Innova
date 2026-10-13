using System.Collections.ObjectModel;
using Innova.Application.Dispatchers;
using Innova.Application.GuestManagement.Guest.Commands.RegisterGuest;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.GuestManagement.Forms
{
    public sealed class RegisterGuestViewModel:FormViewModel
    {
        private readonly IModalService _modalService;
        private DateTime? _dateOfBirth;
        private string _email = string.Empty;

        private string _firstName = string.Empty;
        private string _identityDocumentNumber = string.Empty;
        private IdentityDocumentType? _identityDocumentType;
        private string _lastName = string.Empty;
        private string? _middleName = string.Empty;
        private string _phoneNumber = string.Empty;

        public RegisterGuestViewModel( CommandDispatcher commandDispatcher,
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
            IsEditMode = false;

            RecalculateSaveEnabled();
        }

        // ── Identity fields ──────────────────────────────────────────
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public string? MiddleName
        {
            get => _middleName;
            set
            {
                if (SetProperty(ref _middleName, value))
                    MarkAsChanged();
            }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (SetProperty(ref _dateOfBirth, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        // ── Contact fields ───────────────────────────────────────────
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (SetProperty(ref _phoneNumber, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public IdentityDocumentType? IdentityDocumentType
        {
            get => _identityDocumentType;
            set
            {
                if (SetProperty(ref _identityDocumentType, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public string IdentityDocumentNumber
        {
            get => _identityDocumentNumber;
            set
            {
                if (SetProperty(ref _identityDocumentNumber, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public ObservableCollection<IdentityDocumentType> IdentityDocumentTypes { get; } = [.. Enum.GetValues<IdentityDocumentType>()];

        // ══════════════════════════════════════════════════════════════
        // RecalculateSaveEnabled — lightweight client-side gate on the
        // Save button. This is NOT a substitute for FluentValidation;
        // it only prevents an obviously-incomplete submit. Real
        // validation still happens application-side and surfaces via
        // ExecuteCommandAsync's ApplicationValidationException handling.
        // ══════════════════════════════════════════════════════════════
        private void RecalculateSaveEnabled()
        {
            IsSaveEnabled =
                !string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(LastName)
                && DateOfBirth.HasValue
                && !string.IsNullOrWhiteSpace(PhoneNumber)
                && !string.IsNullOrWhiteSpace(Email)
                && IdentityDocumentType.HasValue
                && !string.IsNullOrWhiteSpace(IdentityDocumentNumber);
        }


        // ══════════════════════════════════════════════════════════════
        // OnSaveAsync — the return type here IS nullable (Guid?), not Guid.
        // ══════════════════════════════════════════════════════════════
        protected override async Task OnSaveAsync()
        {
            RegisterGuestCommand cmd = new(
                FirstName,
                LastName,
                MiddleName,
                DateOnly.FromDateTime(DateOfBirth!.Value),
                PhoneNumber,
                Email,
                IdentityDocumentType!.Value,
                IdentityDocumentNumber);

            await ExecuteCommandAsync<RegisterGuestCommand, Guid>(cmd);

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
