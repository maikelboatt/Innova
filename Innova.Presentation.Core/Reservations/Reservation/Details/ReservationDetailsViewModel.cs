using Innova.Application.Dispatchers;
using Innova.Application.Reservations.DTO;
using Innova.Application.Reservations.Queries;
using Innova.Application.Reservations.Reservation.Commands.CancelReservation;
using Innova.Application.Reservations.Reservation.Commands.ConfirmReservation;
using Innova.Application.Reservations.Reservation.Commands.MarkNoShow;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.RoomInventory.RoomType.Details;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Reservations.Reservation.Details
{
    public sealed class ReservationDetailsViewModel:DetailsViewModel<Guid>
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly QueryDispatcher _queryDispatcher;
        private DateOnly _checkIn;
        private DateOnly _checkOut;
        private Guid? _groupBookingId;

        private Guid _guestId;
        private string _guestName;
        private decimal _nightlyRateAmount;
        private string _nightlyRateCurrency;
        private string _roomTypeName;
        private Guid _roomTypeRequested;
        private string _status;


        public ReservationDetailsViewModel( CommandDispatcher commandDispatcher,
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

            OpenReservationEditCommand = new MvxAsyncCommand(OnOpenReservationEdit);
            OpenRoomTypeCommand = new MvxAsyncCommand(OnOpenRoomType);
            OpenAssociatedGroupBookingCommand = new MvxAsyncCommand(OnOpenAssociatedGroupBooking);
            ConfirmReservationCommand = new MvxAsyncCommand(OnConfirmReservation);
            CancelReservationCommand = new MvxAsyncCommand(OnCancelReservation);
            MarkReservationNoShowCommand = new MvxAsyncCommand(OnMarkReservationNoShow);
        }

        public bool CanConfirmReservation => Status is "Tentative";
        public bool CanCancelReservation => Status is "Tentative" or "Confirmed";
        public bool CanMarkReservationNoShow => Status is "Confirmed";

        public IMvxAsyncCommand OpenReservationEditCommand { get; }
        public IMvxAsyncCommand OpenRoomTypeCommand { get; }
        public IMvxAsyncCommand OpenAssociatedGroupBookingCommand { get; }
        public IMvxAsyncCommand ConfirmReservationCommand { get; }
        public IMvxAsyncCommand CancelReservationCommand { get; }
        public IMvxAsyncCommand MarkReservationNoShowCommand { get; }

        public DateOnly CheckIn
        {
            get => _checkIn;
            set => SetProperty(ref _checkIn, value);
        }
        public DateOnly CheckOut
        {
            get => _checkOut;
            set => SetProperty(ref _checkOut, value);
        }
        public Guid? GroupBookingId
        {
            get => _groupBookingId;
            set => SetProperty(ref _groupBookingId, value);
        }
        public Guid GuestId
        {
            get => _guestId;
            set => SetProperty(ref _guestId, value);
        }
        public string GuestName
        {
            get => _guestName;
            set => SetProperty(ref _guestName, value);
        }
        public decimal NightlyRateAmount
        {
            get => _nightlyRateAmount;
            set => SetProperty(ref _nightlyRateAmount, value);
        }
        public string NightlyRateCurrency
        {
            get => _nightlyRateCurrency;
            set => SetProperty(ref _nightlyRateCurrency, value);
        }
        public string RoomTypeName
        {
            get => _roomTypeName;
            set => SetProperty(ref _roomTypeName, value);
        }
        public Guid RoomTypeRequested
        {
            get => _roomTypeRequested;
            set => SetProperty(ref _roomTypeRequested, value);
        }
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string StatusLabel => Status;

        public string StatusBadge => Status switch
                                     {
                                         "Tentative"  => "Warning",
                                         "Confirmed"  => "Success",
                                         "CheckedIn"  => "Success",
                                         "CheckedOut" => "Neutral",
                                         "Cancelled"  => "Danger",
                                         "NoShow"     => "Danger",
                                         _            => "Neutral"
                                     };

        private async Task OnOpenReservationEdit()
        {
            // await ModalService.NavigateAsync<ReservationEditViewModel, Guid>(Key);
        }

        private async Task OnOpenAssociatedGroupBooking()
        {
            if (!GroupBookingId.HasValue || GroupBookingId.Value == Guid.Empty)
            {
                Notifications.ShowWarning(
                    $"Reservation {Key} is not associated with a group booking.");

            }

            // await ModalService.NavigateAsync<GroupBookingDetailsViewModel, Guid>( GroupBookingId.Value);
        }

        private async Task OnConfirmReservation()
        {
            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Confirm Reservation",
                                 "Are you sure you want to confirm this reservation?");

            if (!confirmed) return;

            await ExecuteCommandAsync(new ConfirmReservationCommand(Key, true));
            await LoadAsync();
            ModalService.Close();
        }

        private async Task OnCancelReservation()
        {
            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Cancel Reservation",
                                 "Are you sure you want to cancel this reservation?");

            if (!confirmed) return;

            await ExecuteCommandAsync(new CancelReservationCommand(Key));
            await LoadAsync();
            ModalService.Close();
        }

        private async Task OnMarkReservationNoShow()
        {
            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Mark Reservation as No Show",
                                 "Are you sure you want to mark this reservation as no show?");

            if (!confirmed) return;

            await ExecuteCommandAsync(new MarkReservationNoShowCommand(Key));
            await LoadAsync();
            ModalService.Close();
        }

        private async Task OnOpenRoomType()
        {
            await ModalService.NavigateAsync<RoomTypeDetailsViewModel, Guid>(RoomTypeRequested);
        }

        protected override async Task LoadAsync()
        {
            ReservationDto dto = await _queryDispatcher.DispatchAsync(new GetReservationByIdQuery(Key));

            GuestId = dto.GuestId;
            RoomTypeRequested = dto.RoomTypeRequested;
            GroupBookingId = dto.GroupBookingId;
            GuestName = dto.GuestName;
            RoomTypeName = dto.RoomTypeName;
            CheckIn = dto.CheckIn;
            CheckOut = dto.CheckOut;
            NightlyRateAmount = dto.NightlyRateAmount;
            NightlyRateCurrency = dto.NightlyRateCurrency;
            Status = dto.Status;

            RaiseAllCanExecuteProperties();
        }

        private void RaiseAllCanExecuteProperties()
        {
            RaisePropertyChanged(nameof(CanConfirmReservation));
            RaisePropertyChanged(nameof(CanCancelReservation));
            RaisePropertyChanged(nameof(CanMarkReservationNoShow));
        }
    }
}
