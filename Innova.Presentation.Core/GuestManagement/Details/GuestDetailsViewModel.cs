using Innova.Application.Dispatchers;
using Innova.Application.GuestManagement.Guest.Commands.DeactivateGuest;
using Innova.Application.GuestManagement.Guest.DTO;
using Innova.Application.GuestManagement.Guest.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.GuestManagement.Details
{
    public sealed class GuestDetailsViewModel:DetailsViewModel<Guid>
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly QueryDispatcher _queryDispatcher;
        private int _age;
        private DateOnly _dateOfBirth;
        private string? _email;

        private string _fullName = string.Empty;
        private string _identityDocumentNumber;
        private string _identityDocumentType;
        private bool _isActive;
        private string _phoneNumber;
        private DateTime _registeredAt;

        public GuestDetailsViewModel( CommandDispatcher commandDispatcher,
                                      IBusyIndicatorService busyIndicator,
                                      INotificationService notifications,
                                      IModalService modalService,
                                      QueryDispatcher queryDispatcher,
                                      IMessageBoxService messageBoxService ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            modalService)
        {
            _queryDispatcher = queryDispatcher;
            _messageBoxService = messageBoxService;
            EditCommand = new MvxAsyncCommand(OnEditAsync);
            DeactivateCommand = new MvxAsyncCommand(OnDeactivateAsync);
        }

        public DateOnly DateOfBirth
        {
            get => _dateOfBirth;
            set => SetProperty(ref _dateOfBirth, value);
        }
        public int Age
        {
            get => _age;
            private set => SetProperty(ref _age, value);
        }

        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public string FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }
        public string IdentityDocumentNumber
        {
            get => _identityDocumentNumber;
            set => SetProperty(ref _identityDocumentNumber, value);
        }
        public string IdentityDocumentType
        {
            get => _identityDocumentType;
            set => SetProperty(ref _identityDocumentType, value);
        }
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }
        public DateTime RegisteredAt
        {
            get => _registeredAt;
            set => SetProperty(ref _registeredAt, value);
        }

        public string StatusLabel => IsActive
                                         ? "Inactive"
                                         : "Active";

        public string StatusBadge => IsActive
                                         ? "Danger"
                                         : "Success";

        public IMvxAsyncCommand EditCommand { get; }
        public IMvxAsyncCommand DeactivateCommand { get; }

        protected override async Task LoadAsync()
        {
            GuestDto dto = await _queryDispatcher.DispatchAsync(new GetGuestByIdQuery(Key));

            FullName = dto.MiddleName is { Length: > 0 }
                           ? $"{dto.FirstName} {dto.MiddleName} {dto.LastName}"
                           : $"{dto.FirstName} {dto.LastName}";

            DateOfBirth = dto.DateOfBirth;
            Age = CalculateAge(dto.DateOfBirth);
            PhoneNumber = dto.PhoneNumber;
            Email = dto.Email;
            IdentityDocumentType = dto.IdentityDocumentType;
            IdentityDocumentNumber = dto.IdentityDocumentNumber;
            IsActive = dto.IsActive;
            RegisteredAt = dto.CreatedAt;
        }

        private async Task OnEditAsync()
        {
            // await ModalService.NavigateAsync<GuestUpdateViewModel, Guid>(Key);
        }

        private static int CalculateAge( DateOnly dateOfBirth )
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - dateOfBirth.Year;
            if (today < dateOfBirth.AddYears(age)) age--;
            return age;
        }

        private async Task OnDeactivateAsync()
        {
            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Unregister Guest",
                                 $"Are you sure you want to unregister {FullName}? This marks the guest as inactive; their records are retained.");

            if (!confirmed)
                return;

            // ExecuteCommandAsync<TCommand> (Unit-returning overload) is fine
            // here — unlike EditGuestViewModel's OnSaveAsync, there's no
            // meaningful "did it succeed" branch beyond what the base
            // exception handling already surfaces via Notifications.
            await ExecuteCommandAsync(new DeactivateGuestCommand(Key));

            ModalService.Close();
        }
    }
}
