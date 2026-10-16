using System.Text.RegularExpressions;
using Innova.Application.Dispatchers;
using Innova.Application.GuestManagement.Guest.Commands.RegisterGuest;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using DocumentType = Innova.Domain.GuestManagement.ValueObjects.IdentityDocumentType;

namespace Innova.Presentation.Core.GuestManagement.Forms
{
    public sealed class RegisterGuestViewModel:FormViewModel
    {
        private readonly IModalService _modalService;
        private readonly INotificationService _notifications;

        private DateTime? _dateOfBirth;
        private string _email = string.Empty;
        private string _firstName = string.Empty;
        private string _identityDocumentNumber = string.Empty;
        private string _identityDocumentType = string.Empty;
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
            _notifications = notifications;
            IsEditMode = false;

            // Built from the enum itself, so the dropdown can never offer something the code cannot parse.
            IdentityDocumentTypes =
            [
                .. Enum
                   .GetValues<DocumentType>()
                   .Select(type => new IdentityDocumentOptionViewModel(type.ToString(), LabelFor(type.ToString())))
            ];
        }

        // ── Dropdown / date picker support ───────────────────────────
        public IReadOnlyList<IdentityDocumentOptionViewModel> IdentityDocumentTypes { get; }

        /// <summary>A date of birth cannot be in the future (the date picker greys those days out).</summary>
        public DateTime MaxDateOfBirth => DateTime.Today;

        /// <summary>Where the calendar opens: a date of birth is rarely "this month".</summary>
        public DateTime CalendarStartDate => DateTime.Today.AddYears(-30);

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

        public string? DateOfBirthError => DateOfBirth is { } date && date.Date > DateTime.Today
                                               ? "Date of birth can't be in the future."
                                               : null;

        public bool HasDateOfBirthError => DateOfBirthError is not null;

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

        /// <summary>Optional: RegisterGuestCommand.Email is nullable and so is the database column.</summary>
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

        public string? EmailError => !string.IsNullOrWhiteSpace(Email) && !IsPlausibleEmail(Email)
                                         ? "Enter a valid email address, or leave it empty."
                                         : null;

        public bool HasEmailError => EmailError is not null;

        // ── Identity document ────────────────────────────────────────
        public string IdentityDocumentType
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

        // ══════════════════════════════════════════════════════════════
        // RecalculateSaveEnabled — lightweight client-side gate on the
        // Save button. This is NOT a substitute for FluentValidation;
        // it only prevents an obviously-incomplete submit. Real
        // validation still happens application-side and surfaces via
        // ExecuteCommandAsync's ApplicationValidationException handling.
        // ══════════════════════════════════════════════════════════════
        private void RecalculateSaveEnabled()
        {
            RaisePropertyChanged(nameof(DateOfBirthError));
            RaisePropertyChanged(nameof(HasDateOfBirthError));
            RaisePropertyChanged(nameof(EmailError));
            RaisePropertyChanged(nameof(HasEmailError));

            IsSaveEnabled =
                !string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(LastName)
                && DateOfBirth.HasValue
                && !HasDateOfBirthError
                && !string.IsNullOrWhiteSpace(PhoneNumber)
                && !HasEmailError // empty is fine, a malformed address is not
                && !string.IsNullOrWhiteSpace(IdentityDocumentType)
                && !string.IsNullOrWhiteSpace(IdentityDocumentNumber);
        }

        protected override async Task OnSaveAsync()
        {
            if (DateOfBirth is not { } dateOfBirth
                || !Enum.TryParse(IdentityDocumentType, out DocumentType documentType))
                return;

            RegisterGuestCommand command = new(
                FirstName.Trim(),
                LastName.Trim(),
                NullIfBlank(MiddleName),
                DateOnly.FromDateTime(dateOfBirth),
                PhoneNumber.Trim(),
                NullIfBlank(Email),
                documentType,
                // Trim + upper-case so "gha-123" and "GHA-123 " are the same document (the unique index is
                // case-insensitive, and the identity-document value object does not always normalise it).
                IdentityDocumentNumber
                    .Trim()
                    .ToUpperInvariant());

            Guid guestId = await ExecuteCommandAsync<RegisterGuestCommand, Guid>(command);

            // ExecuteCommandAsync has already shown the error (for example "a guest with this identity document
            // is already registered"). Stay open so nothing the user typed is lost.
            if (guestId == Guid.Empty) return;

            HasUnsavedChanges = false;
            _notifications.ShowSuccess($"{FirstName.Trim()} {LastName.Trim()} registered.");
            _modalService.Close();
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();
            return Task.CompletedTask;
        }

        // ── Helpers ──────────────────────────────────────────────────
        private static string? NullIfBlank( string? value ) => string.IsNullOrWhiteSpace(value)
                                                                   ? null
                                                                   : value.Trim();

        private static bool IsPlausibleEmail( string value ) => Regex.IsMatch(value.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        private static string LabelFor( string name ) => name switch
                                                         {
                                                             "NationalId"     => "National ID",
                                                             "GhanaCard"      => "Ghana Card",
                                                             "DriversLicense" => "Driver's license",
                                                             "VoterId"        => "Voter's ID",
                                                             "VotersId"       => "Voter's ID",
                                                             _                => Regex.Replace(name, "(?<!^)([A-Z])", " $1")
                                                         };
    }
}
