using System.Collections.ObjectModel;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Dispatchers;
using Innova.Application.GuestManagement.Guest.Commands.ChangeGuestIdentityDocument;
using Innova.Application.GuestManagement.Guest.Commands.UpdateGuestContactDetails;
using Innova.Application.GuestManagement.Guest.DTO;
using Innova.Application.GuestManagement.Guest.Queries;
using Innova.Domain.GuestManagement.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.GuestManagement.Forms
{
    public sealed class UpdateGuestViewModel:FormViewModel, IMvxViewModel<Guid>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;
        private DateTime? _dateOfBirth;
        private string? _email = string.Empty;

        private string _fullName = string.Empty;
        public Guid _guestId;
        private string _identityDocumentNumber = string.Empty;
        private IdentityDocumentType? _identityDocumentType;
        private string _lastName = string.Empty;
        private string? _middleName = string.Empty;
        private string _phoneNumber = string.Empty;

        public UpdateGuestViewModel( CommandDispatcher commandDispatcher,
                                     IBusyIndicatorService busyIndicator,
                                     INotificationService notifications,
                                     IMessageBoxService messageBox,
                                     QueryDispatcher queryDispatcher,
                                     IModalService modalService ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            messageBox)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;
            IsEditMode = true;
        }

        // ── Read-only identity display ───────────────────────────────
        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetProperty(ref _fullName, value))
                {
                    MarkAsChanged();
                }
            }
        }

        // ── Editable fields ───────────────────────────────────────────
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (SetProperty(ref _phoneNumber, value))
                {
                    MarkAsChanged();
                }
            }
        }

        public string? Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    MarkAsChanged();
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
                }
            }
        }

        public ObservableCollection<IdentityDocumentType> IdentityDocumentTypes { get; } = [.. Enum.GetValues<IdentityDocumentType>()];


        public void Prepare( Guid parameter )
        {
            _guestId = parameter;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            try
            {
                BusyIndicator.Show();

                GuestDto dto = await _queryDispatcher.DispatchAsync(new GetGuestByIdQuery(_guestId));

                FullName = dto.MiddleName is { Length: > 0 }
                               ? $"{dto.FirstName} {dto.MiddleName} {dto.LastName}"
                               : $"{dto.FirstName} {dto.LastName}";

                PhoneNumber = dto.PhoneNumber;
                Email = dto.Email;
                IdentityDocumentType = Enum.Parse<IdentityDocumentType>(dto.IdentityDocumentType);
                IdentityDocumentNumber = dto.IdentityDocumentNumber;

                HasUnsavedChanges = false;
            }
            finally
            {
                BusyIndicator.Hide();

            }
        }

        protected override async Task OnSaveAsync()
        {
            if (!IdentityDocumentType.HasValue)
                return;

            UpdateGuestContactDetailsCommand contactCommand = new(
                _guestId,
                PhoneNumber,
                Email);

            ChangeGuestIdentityDocumentCommand documentCommand = new(
                _guestId,
                IdentityDocumentType.Value,
                IdentityDocumentNumber);

            await ExecuteCommandAsync<UpdateGuestContactDetailsCommand, Unit>(contactCommand);

            await ExecuteCommandAsync<ChangeGuestIdentityDocumentCommand, Unit>(documentCommand);

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
