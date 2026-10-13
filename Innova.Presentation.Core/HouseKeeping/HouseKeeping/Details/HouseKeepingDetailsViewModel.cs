using Innova.Application.Dispatchers;
using Innova.Application.HouseKeeping.HouseKeeping.DTO;
using Innova.Application.HouseKeeping.HouseKeeping.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.RoomInventory.Room.Details;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping.Details
{
    public sealed class HouseKeepingDetailsViewModel:DetailsViewModel<Guid>
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly QueryDispatcher _queryDispatcher;
        private Guid? _assignedStaffId;
        private DateTime? _completedAt;
        private DateTime _createdAt;
        private string? _inspectionNotes;
        private bool? _inspectionPassed;

        private Guid _roomId;
        private string _status;
        private string _type;

        public HouseKeepingDetailsViewModel( CommandDispatcher commandDispatcher,
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

            AssignHouseKeepingStaffCommand = new MvxAsyncCommand(OnAssignHouseKeepingStaff);
            OpenRoomDetailsCommand = new MvxAsyncCommand(OnOpenRoomType);
            CompleteHouseKeepingCommand = new MvxAsyncCommand(OnCompleteHouseKeeping);
        }

        public Guid? AssignedStaffId
        {
            get => _assignedStaffId;
            set => SetProperty(ref _assignedStaffId, value);
        }
        public DateTime? CompletedAt
        {
            get => _completedAt;
            set => SetProperty(ref _completedAt, value);
        }
        public DateTime CreatedAt
        {
            get => _createdAt;
            set => SetProperty(ref _createdAt, value);
        }
        public string? InspectionNotes
        {
            get => _inspectionNotes;
            set => SetProperty(ref _inspectionNotes, value);
        }
        public bool? InspectionPassed
        {
            get => _inspectionPassed;
            set => SetProperty(ref _inspectionPassed, value);
        }
        public Guid RoomId
        {
            get => _roomId;
            set => SetProperty(ref _roomId, value);
        }
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public IMvxAsyncCommand AssignHouseKeepingStaffCommand { get; }
        public IMvxAsyncCommand OpenRoomDetailsCommand { get; }
        public IMvxAsyncCommand CompleteHouseKeepingCommand { get; }

        private async Task OnAssignHouseKeepingStaff()
        {
            // await ModalService.NavigateAsync<HouseKeepingAssignDialogViewModel, Guid>(Key);
        }

        private async Task OnOpenRoomType()
        {
            await ModalService.NavigateAsync<RoomDetailsViewModel, Guid>(RoomId);
        }

        private async Task OnCompleteHouseKeeping()
        {
            // await ModalService.NavigateAsync<HouseKeepingCompleteDialogViewModel, Guid>(Key);

        }

        protected override async Task LoadAsync()
        {
            HouseKeepingTaskDto dto = await _queryDispatcher.DispatchAsync(new GetHouseKeepingTaskByIdQuery(Key));

            RoomId = dto.RoomId;
            AssignedStaffId = dto.AssignedStaffId;
            Type = dto.Type;
            CreatedAt = dto.CreatedAt;
            CompletedAt = dto.CompletedAt;
            Status = dto.Status;
            InspectionNotes = dto.InspectionNotes;
            InspectionPassed = dto.InspectionPassed;
        }
    }
}
