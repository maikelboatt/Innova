using Innova.Application.Dispatchers;
using Innova.Application.FrontDesk.Stay.Commands.CheckOut;
using Innova.Application.FrontDesk.Stay.DTO;
using Innova.Application.FrontDesk.Stay.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Reservations.Reservation.Details;
using Innova.Presentation.Core.RoomInventory.Room.Details;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.FrontDesk.Stay.Details
{
    public sealed class StayDetailsViewModel:DetailsViewModel<Guid>
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly QueryDispatcher _queryDispatcher;
        private DateTime? _actualCheckIn;
        private DateTime? _actualCheckOut;
        private Guid? _groupBookingId;
        private int _maxOccupancy;
        private IReadOnlyCollection<Guid> _occupants = Array.Empty<Guid>();

        private Guid _reservationId;
        private Guid _roomId;
        private string _roomNumber = string.Empty;

        public StayDetailsViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            IModalService modalService,
            QueryDispatcher queryDispatcher,
            IMessageBoxService messageBoxService )
            :base(
                commandDispatcher,
                busyIndicator,
                notifications,
                modalService)
        {
            _queryDispatcher = queryDispatcher;
            _messageBoxService = messageBoxService;

            OpenReservationCommand = new MvxAsyncCommand(OnOpenReservation);
            OpenRoomCommand = new MvxAsyncCommand(OnOpenRoom);
            CheckOutCommand = new MvxAsyncCommand(OnCheckOut);
        }

        // ── Stay information ─────────────────────────────────────
        public Guid ReservationId
        {
            get => _reservationId;
            private set => SetProperty(ref _reservationId, value);
        }
        public Guid RoomId
        {
            get => _roomId;
            private set => SetProperty(ref _roomId, value);
        }
        public string RoomNumber
        {
            get => _roomNumber;
            private set => SetProperty(ref _roomNumber, value);
        }
        public int MaxOccupancy
        {
            get => _maxOccupancy;
            private set => SetProperty(ref _maxOccupancy, value);
        }
        public IReadOnlyCollection<Guid> Occupants
        {
            get => _occupants;
            private set
            {
                if (SetProperty(ref _occupants, value))
                    RaisePropertyChanged(nameof(OccupantCount));
            }
        }
        public int OccupantCount => Occupants.Count;

        public DateTime? ActualCheckIn
        {
            get => _actualCheckIn;
            private set => SetProperty(ref _actualCheckIn, value);
        }
        public DateTime? ActualCheckOut
        {
            get => _actualCheckOut;
            private set
            {
                if (SetProperty(ref _actualCheckOut, value))
                {
                    RaisePropertyChanged(nameof(IsCheckedOut));
                    RaisePropertyChanged(nameof(CanCheckOut));
                }
            }
        }
        public Guid? GroupBookingId
        {
            get => _groupBookingId;
            private set => SetProperty(ref _groupBookingId, value);
        }

        public bool IsCheckedOut => ActualCheckOut.HasValue;

        public bool CanCheckOut => ActualCheckIn.HasValue &&
                                   !ActualCheckOut.HasValue;

        public IMvxAsyncCommand OpenReservationCommand { get; }
        public IMvxAsyncCommand OpenRoomCommand { get; }
        public IMvxAsyncCommand CheckOutCommand { get; }

        protected override async Task LoadAsync()
        {
            StayDto dto = await _queryDispatcher.DispatchAsync(new GetStayByIdQuery(Key));

            ReservationId = dto.ReservationId;
            RoomId = dto.RoomId;
            RoomNumber = dto.RoomNumber;
            MaxOccupancy = dto.MaxOccupancy;
            Occupants = dto.Occupants;
            ActualCheckIn = dto.ActualCheckIn;
            ActualCheckOut = dto.ActualCheckOut;
            GroupBookingId = dto.GroupBookingId;

            await RaisePropertyChanged(nameof(IsCheckedOut));
            await RaisePropertyChanged(nameof(CanCheckOut));
        }

        private async Task OnOpenReservation()
        {
            await ModalService.NavigateAsync<ReservationDetailsViewModel, Guid>(ReservationId);
        }

        private async Task OnOpenRoom()
        {
            await ModalService.NavigateAsync<RoomDetailsViewModel, Guid>(RoomId);
        }

        private async Task OnCheckOut()
        {
            if (!CanCheckOut)
                return;

            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Check Out",
                                 $"Are you sure you want to check out the stay in room {RoomNumber}?");

            if (!confirmed)
                return;

            await ExecuteCommandAsync<CheckOutCommand, Guid>(new CheckOutCommand(Key));

            await LoadAsync();

            ModalService.Close();
        }
    }
}
