using Innova.Application.Dispatchers;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.ScheduleTask;
using Innova.Domain.HouseKeeping.ValueObjects;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.HouseKeeping.HouseKeeping.Forms
{
    public sealed class ScheduleHouseKeepingViewModel:FormViewModel, IMvxViewModel<ScheduleHouseKeepingParameter>
    {
        private readonly IModalService _modalService;

        private Guid _roomId;
        private HouseKeepingTaskType _type;

        public ScheduleHouseKeepingViewModel( CommandDispatcher commandDispatcher,
                                              IBusyIndicatorService busyIndicator,
                                              INotificationService notifications,
                                              IMessageBoxService messageBox,
                                              IModalService modalService ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            messageBox) => _modalService = modalService;

        public Guid RoomId
        {
            get => _roomId;
            private set => SetProperty(ref _roomId, value);
        }

        public HouseKeepingTaskType Type
        {
            get => _type;
            private set => SetProperty(ref _type, value);
        }

        public void Prepare( ScheduleHouseKeepingParameter parameter )
        {
            _roomId = parameter.RoomId;
            _type = parameter.Type;
        }

        protected override async Task OnSaveAsync()
        {
            await ExecuteCommandAsync<ScheduleTaskCommand, Guid>(new ScheduleTaskCommand(_roomId, _type));

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
