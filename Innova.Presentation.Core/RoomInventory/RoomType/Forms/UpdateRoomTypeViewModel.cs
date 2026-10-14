using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Application.RoomInventory.RoomTypeDefinition.Commands.UpdateRoomType;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.RoomInventory.RoomType.Forms
{
    public sealed class UpdateRoomTypeViewModel:FormViewModel, IMvxViewModel<Guid>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;
        private decimal _baseRateAmount;
        private int _maxOccupancy;

        private string _roomName = string.Empty;
        private Guid _roomTypeId;

        public UpdateRoomTypeViewModel( CommandDispatcher commandDispatcher,
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

        public decimal BaseRateAmount
        {
            get => _baseRateAmount;
            set
            {
                if (SetProperty(ref _baseRateAmount, value))
                {
                    MarkAsChanged();
                }
            }
        }
        public int MaxOccupancy
        {
            get => _maxOccupancy;
            set
            {
                if (SetProperty(ref _maxOccupancy, value))
                {
                    MarkAsChanged();
                }
            }
        }
        public string RoomName
        {
            get => _roomName;
            set
            {
                if (SetProperty(ref _roomName, value))
                {
                    MarkAsChanged();
                }
            }
        }

        public void Prepare( Guid parameter )
        {
            _roomTypeId = parameter;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            try
            {
                BusyIndicator.Show();

                RoomTypeDto dto = await _queryDispatcher.DispatchAsync(new GetRoomTypeByIdQuery(_roomTypeId));

                RoomName = dto.Name;
                MaxOccupancy = dto.MaxOccupancy;
                BaseRateAmount = dto.BaseRateAmount;

                HasUnsavedChanges = false;
            }
            finally
            {
                BusyIndicator.Hide();
            }
        }

        protected override async Task OnSaveAsync()
        {
            UpdateRoomTypeCommand cmd = new(
                _roomTypeId,
                RoomName,
                MaxOccupancy,
                BaseRateAmount);

            await ExecuteCommandAsync(cmd);
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
