using Innova.Application.Dispatchers;
using Innova.Application.Reservations.Reservation.Commands.BookReservation;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Application.Settings;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Reservations.GroupBooking.Forms;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Reservations.Reservation.Forms
{
    public sealed class BookReservationViewModel:FormViewModel, IMvxViewModel<Guid>
    {
        private readonly HotelSettings _hotelSettings;
        private readonly IModalService _modalService;
        private readonly INotificationService _notifications;
        private readonly QueryDispatcher _queryDispatcher;
        private DateTime? _checkInDate = DateTime.Today;
        private DateTime? _checkOutDate = DateTime.Today.AddDays(1);

        private string _currency = string.Empty;

        private Guid _guestId;

        private bool _isLoadingRoomTypes;
        private bool _isSaving;

        private IReadOnlyList<RoomTypeDto> _roomTypes = [];
        private RoomTypeDto? _selectedRoomType;

        public BookReservationViewModel(
            CommandDispatcher commandDispatcher,
            QueryDispatcher queryDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            IMessageBoxService messageBox,
            IModalService modalService,
            HotelSettings hotelSettings )
            :base(
                commandDispatcher,
                busyIndicator,
                notifications,
                messageBox)
        {
            _queryDispatcher = queryDispatcher;
            _notifications = notifications;
            _modalService = modalService;
            _hotelSettings = hotelSettings;

            IsEditMode = false;

            ToGroupBookingCommand = new MvxAsyncCommand(ToGroupBookingAsync);
        }

        // ── Guest ────────────────────────────────────────────────

        public Guid GuestId
        {
            get => _guestId;
            private set => SetProperty(ref _guestId, value);
        }

        // ── Room types ───────────────────────────────────────────

        public IReadOnlyList<RoomTypeDto> RoomTypes
        {
            get => _roomTypes;
            private set
            {
                if (SetProperty(ref _roomTypes, value))
                    RecalculateSaveEnabled();
            }
        }

        public RoomTypeDto? SelectedRoomType
        {
            get => _selectedRoomType;
            set
            {
                if (!SetProperty(ref _selectedRoomType, value))
                    return;

                RaisePropertyChanged(nameof(NightlyRateAmount));
                RaisePropertyChanged(nameof(NightlyRateCurrency));
                RaisePropertyChanged(nameof(EstimatedTotal));
                RaisePropertyChanged(nameof(EstimatedTotalDisplay));

                RecalculateSaveEnabled();
            }
        }

        public bool IsLoadingRoomTypes
        {
            get => _isLoadingRoomTypes;
            private set
            {
                if (SetProperty(ref _isLoadingRoomTypes, value))
                    RecalculateSaveEnabled();
            }
        }

        // ── Dates ────────────────────────────────────────────────

        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set
            {
                if (!SetProperty(ref _checkInDate, value))
                    return;

                RaisePropertyChanged(nameof(NumberOfNights));
                RaisePropertyChanged(nameof(EstimatedTotal));
                RaisePropertyChanged(nameof(EstimatedTotalDisplay));

                RecalculateSaveEnabled();
            }
        }

        public DateTime? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                if (!SetProperty(ref _checkOutDate, value))
                    return;

                RaisePropertyChanged(nameof(NumberOfNights));
                RaisePropertyChanged(nameof(EstimatedTotal));
                RaisePropertyChanged(nameof(EstimatedTotalDisplay));

                RecalculateSaveEnabled();
            }
        }

        public int NumberOfNights => CheckInDate.HasValue && CheckOutDate.HasValue
                                         ? (CheckOutDate.Value.Date - CheckInDate.Value.Date).Days
                                         : 0;

        // ── Rate and currency ────────────────────────────────────

        public decimal NightlyRateAmount => SelectedRoomType?.BaseRateAmount ?? 0m;

        public string NightlyRateCurrency => SelectedRoomType?.BaseRateCurrency ?? Currency;

        public string Currency
        {
            get => _currency;
            set
            {
                if (!SetProperty(ref _currency, value))
                    return;

                RaisePropertyChanged(nameof(CurrencyWarning));
                RaisePropertyChanged(nameof(HasCurrencyWarning));
                RaisePropertyChanged(nameof(ValidationMessage));
                RaisePropertyChanged(nameof(HasValidationMessage));
                RaisePropertyChanged(nameof(EstimatedTotalDisplay));

                RecalculateSaveEnabled();
            }
        }

        public string? CurrencyWarning => IsValidCurrency &&
                                          !string.Equals(
                                              Currency.Trim(),
                                              _hotelSettings.DefaultCurrency,
                                              StringComparison.OrdinalIgnoreCase)
                                              ? $"The hotel's default currency is {_hotelSettings.DefaultCurrency}. " +
                                                "Changing the currency does not convert the room rate."
                                              : null;

        public bool HasCurrencyWarning => CurrencyWarning is not null;

        public decimal EstimatedTotal => NumberOfNights > 0
                                             ? NightlyRateAmount * NumberOfNights
                                             : 0m;

        public string EstimatedTotalDisplay => $"{Currency.Trim().ToUpperInvariant()} {EstimatedTotal:N2}";

        public IMvxAsyncCommand ToGroupBookingCommand { get; }


        // ── Validation ───────────────────────────────────────────

        public string? ValidationMessage
        {
            get
            {
                if (GuestId == Guid.Empty)
                    return "A guest must be selected before booking.";

                if (SelectedRoomType is null)
                    return "Select a room type.";

                if (!CheckInDate.HasValue)
                    return "Select a check-in date.";

                if (!CheckOutDate.HasValue)
                    return "Select a check-out date.";

                if (CheckInDate.Value.Date < DateTime.Today)
                    return "Check-in cannot be in the past.";

                if (CheckOutDate.Value.Date <= CheckInDate.Value.Date)
                    return "Check-out must be after check-in.";

                if (!IsValidCurrency)
                    return "Enter a 3-letter currency code, for example GHS.";

                if (NightlyRateAmount <= 0)
                    return "The selected room type must have a valid nightly rate.";

                if (!string.Equals(
                        Currency.Trim(),
                        NightlyRateCurrency,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return $"The selected room rate is in {NightlyRateCurrency}. " +
                           "The reservation currency must match the room rate currency.";
                }

                return null;
            }
        }

        public bool HasValidationMessage => ValidationMessage is not null;

        private bool IsValidCurrency => Currency.Trim()
                                                .Length == 3 &&
                                        Currency
                                            .Trim()
                                            .All(char.IsLetter);

        public void Prepare( Guid parameter )
        {
            GuestId = parameter;
        }

        // ── Lifecycle ────────────────────────────────────────────

        public override async Task Initialize()
        {
            await base.Initialize();

            Currency = _hotelSettings.DefaultCurrency;

            await LoadRoomTypesAsync();
        }

        private async Task ToGroupBookingAsync()
        {
            await _modalService.ShowAsync<OpenGroupBookingViewModel, Guid>(GuestId);
        }

        private async Task LoadRoomTypesAsync()
        {
            IsLoadingRoomTypes = true;

            try
            {
                IReadOnlyCollection<RoomTypeDto> roomTypes = await _queryDispatcher.DispatchAsync(new ListRoomTypesQuery());

                RoomTypes = roomTypes.ToList();
            }
            catch (Exception exception)
            {
                _notifications.ShowError(
                    $"Could not load room types: {exception.Message}");
            }
            finally
            {
                IsLoadingRoomTypes = false;
            }
        }

        // ── Save validation ──────────────────────────────────────

        private void RecalculateSaveEnabled()
        {
            IsSaveEnabled =
                !IsLoadingRoomTypes &&
                !_isSaving &&
                ValidationMessage is null;

            RaisePropertyChanged(nameof(ValidationMessage));
            RaisePropertyChanged(nameof(HasValidationMessage));
            RaisePropertyChanged(nameof(CurrencyWarning));
            RaisePropertyChanged(nameof(HasCurrencyWarning));
            RaisePropertyChanged(nameof(EstimatedTotal));
            RaisePropertyChanged(nameof(EstimatedTotalDisplay));
        }

        // ── Actions ──────────────────────────────────────────────

        protected override async Task OnSaveAsync()
        {
            if (ValidationMessage is not null ||
                SelectedRoomType is null ||
                !CheckInDate.HasValue ||
                !CheckOutDate.HasValue)
            {
                RecalculateSaveEnabled();
                return;
            }

            _isSaving = true;
            RecalculateSaveEnabled();

            try
            {
                BookReservationCommand command = new(
                    GuestId,
                    SelectedRoomType.RoomTypeId,
                    DateOnly.FromDateTime(CheckInDate.Value.Date),
                    DateOnly.FromDateTime(CheckOutDate.Value.Date),
                    NightlyRateAmount,
                    NightlyRateCurrency,
                    _hotelSettings.CancellationWindowHours,
                    _hotelSettings.CancellationFeeAmount,
                    _hotelSettings.DefaultCurrency,
                    null);

                Guid reservationId = await ExecuteCommandAsync<BookReservationCommand, Guid>(command);

                if (reservationId == Guid.Empty)
                    return;

                HasUnsavedChanges = false;

                _notifications.ShowSuccess(
                    $"Reservation booked successfully. " +
                    $"Reference: {reservationId}");

                _modalService.Close();
            }
            finally
            {
                _isSaving = false;
                RecalculateSaveEnabled();
            }
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();

            return Task.CompletedTask;
        }
    }
}
