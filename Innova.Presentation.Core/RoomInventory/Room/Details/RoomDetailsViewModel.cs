using System.Collections.ObjectModel;
using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Application.RoomInventory.Room.Commands.ReturnRoomToService;
using Innova.Application.RoomInventory.Room.Commands.TakeRoomOutOfService;
using Innova.Domain.HouseKeeping.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.FrontDesk.Stay.Details;
using Innova.Presentation.Core.HouseKeeping.HouseKeeping.Forms;
using Innova.Presentation.Core.RoomInventory.RoomType.Details;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.RoomInventory.Room.Details
{
    public sealed class RoomDetailsViewModel:DetailsViewModel<Guid>
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly QueryDispatcher _queryDispatcher;
        private int _floorLevel;
        private string? _floorWing;

        private string _roomNumber;
        private Guid _roomTypeId;
        private string _roomTypeName;
        private HouseKeepingTaskType _selectedType = HouseKeepingTaskType.Cleaning;

        private bool _showTaskType;
        private string _status;

        public RoomDetailsViewModel( CommandDispatcher commandDispatcher,
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

            ReturnRoomToServiceCommand = new MvxAsyncCommand(OnReturnRoomToService);
            TakeRoomOutOfServiceCommand = new MvxAsyncCommand(OnTakeRoomOutOfService);
            GetActiveStayCommand = new MvxAsyncCommand(OnGetActiveStay);
            OpenRoomTypeCommand = new MvxAsyncCommand(OnOpenRoomType);
            ShowTaskTypeCommand = new MvxAsyncCommand(OnShowTaskType);
            ScheduleHouseKeepingCommand = new MvxAsyncCommand(OnOpenHouseKeepingSchedule);
        }

        public bool ShowTaskType
        {
            get => _showTaskType;
            set => SetProperty(ref _showTaskType, value);
        }

        public HouseKeepingTaskType SelectedType
        {
            get => _selectedType;
            set => SetProperty(ref _selectedType, value);
        }

        public ObservableCollection<HouseKeepingTaskType> TaskTypes { get; } = [.. Enum.GetValues<HouseKeepingTaskType>()];


        // ── Identity ────────────────────────────────────────────
        public string RoomNumber
        {
            get => _roomNumber;
            set => SetProperty(ref _roomNumber, value);
        }
        public string RoomTypeName
        {
            get => _roomTypeName;
            set => SetProperty(ref _roomTypeName, value);
        }
        public Guid RoomTypeId
        {
            get => _roomTypeId;
            set => SetProperty(ref _roomTypeId, value);
        }

        // ── Location ────────────────────────────────────────────
        public int FloorLevel
        {
            get => _floorLevel;
            set => SetProperty(ref _floorLevel, value);
        }
        public string? FloorWing
        {
            get => _floorWing;
            set => SetProperty(ref _floorWing, value);
        }

        public string FloorDisplay => string.IsNullOrWhiteSpace(FloorWing)
                                          ? $"Floor {FloorLevel}"
                                          : $"Floor {FloorLevel} · {FloorWing}";

        // ── Status ───────────────────────────────────────────────
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }

        public string StatusLabel => Status;

        public string StatusBadge => Status switch
                                     {
                                         "Occupied"     => "Success",
                                         "Vacant"       => "Neutral",
                                         "Dirty"        => "Warning",
                                         "OutOfService" => "Danger",
                                         _              => "Neutral"
                                     };

        public bool CanMarkOutOfService => Status is "Vacant" or "Dirty";
        public bool CanReturnToService => Status is "Occupied" or "OutOfService";

        public IMvxAsyncCommand ReturnRoomToServiceCommand { get; }
        public IMvxAsyncCommand TakeRoomOutOfServiceCommand { get; }
        public IMvxAsyncCommand GetActiveStayCommand { get; }
        public IMvxAsyncCommand OpenRoomTypeCommand { get; }
        public IMvxAsyncCommand ShowTaskTypeCommand { get; }
        public IMvxAsyncCommand ScheduleHouseKeepingCommand { get; }

        private Task OnShowTaskType()
        {
            ShowTaskType = true;
            return Task.CompletedTask;
        }

        private async Task OnReturnRoomToService()
        {
            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Return Room To Service",
                                 "Are you sure you want to return this room to Service?");

            if (!confirmed) return;

            await ExecuteCommandAsync(new ReturnRoomToServiceCommand(Key));
            await LoadAsync();
            ModalService.Close();
        }

        private async Task OnTakeRoomOutOfService()
        {
            bool confirmed = await _messageBoxService.ShowConfirmAsync(
                                 "Take Out Of Service",
                                 "Are you sure you want to take this room out of Service?");

            if (!confirmed) return;

            await ExecuteCommandAsync(new TakeRoomOutOfServiceCommand(Key));
            await LoadAsync();
            ModalService.Close();
        }

        private async Task OnGetActiveStay()
        {
            await ModalService.NavigateAsync<StayDetailsViewModel, Guid>(Key);
        }

        private async Task OnOpenRoomType()
        {
            await ModalService.NavigateAsync<RoomTypeDetailsViewModel, Guid>(RoomTypeId);
        }

        private async Task OnOpenHouseKeepingSchedule()
        {
            ScheduleHouseKeepingParameter param = new(Key, SelectedType);
            await ModalService.NavigateAsync<ScheduleHouseKeepingViewModel, ScheduleHouseKeepingParameter>(param);
        }

        protected override async Task LoadAsync()
        {
            RoomDto dto = await _queryDispatcher.DispatchAsync(new GetRoomByIdQuery(Key));

            RoomTypeName = dto.RoomTypeName;
            RoomTypeId = dto.RoomTypeId;
            RoomNumber = dto.RoomNumber;
            FloorLevel = dto.FloorLevel;
            FloorWing = dto.FloorWing;
            Status = dto.Status;

            RaiseAllCanExecuteProperties();
        }

        private void RaiseAllCanExecuteProperties()
        {
            RaisePropertyChanged(nameof(CanMarkOutOfService));
            RaisePropertyChanged(nameof(CanReturnToService));
        }
    }
}
