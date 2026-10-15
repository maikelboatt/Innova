using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.Room.Commands.CreateRoom;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.RoomInventory.Room.Forms
{
    public sealed class CreateRoomViewModel:FormViewModel, IMvxViewModel<Guid>
    {
        private readonly IModalService _modalService;
        private readonly INotificationService _notifications;
        private int _floorLevel;

        private string _roomNumber = string.Empty;
        private Guid _roomTypeId;
        private string? _wing = string.Empty;

        public CreateRoomViewModel( CommandDispatcher commandDispatcher,
                                    IBusyIndicatorService busyIndicator,
                                    INotificationService notifications,
                                    IMessageBoxService messageBox,
                                    IModalService modalService ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            messageBox)
        {
            _notifications = notifications;
            _modalService = modalService;

            IsEditMode = false;
        }

        public int FloorLevel
        {
            get => _floorLevel;
            set
            {
                if (SetProperty(ref _floorLevel, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }
        public string RoomNumber
        {
            get => _roomNumber;
            set
            {
                if (SetProperty(ref _roomNumber, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }
        public string? Wing
        {
            get => _wing;
            set
            {
                if (SetProperty(ref _wing, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public void Prepare( Guid parameter )
        {
            _roomTypeId = parameter;
        }

        private void RecalculateSaveEnabled()
        {
            IsSaveEnabled = !string.IsNullOrWhiteSpace(RoomNumber)
                            && !string.IsNullOrWhiteSpace(Wing)
                            && FloorLevel > 0;
        }

        protected override async Task OnSaveAsync()
        {
            CreateRoomCommand cmd = new(
                RoomNumber,
                FloorLevel,
                Wing,
                _roomTypeId);

            Guid roomId = await ExecuteCommandAsync<CreateRoomCommand, Guid>(cmd);
            if (roomId == Guid.Empty) return;
            HasUnsavedChanges = false;
            _notifications.ShowSuccess($"{RoomNumber.Trim()} registered.");
            _modalService.Close();
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();
            return Task.CompletedTask;
        }
    }
}
