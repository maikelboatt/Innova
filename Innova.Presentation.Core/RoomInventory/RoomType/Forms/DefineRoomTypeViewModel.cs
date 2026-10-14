using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.RoomTypeDefinition.Commands.DefineRoomType;
using Innova.Application.Settings;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.Core.RoomInventory.RoomType.Forms
{
    public sealed class DefineRoomTypeViewModel:FormViewModel
    {
        private readonly string _defaultCurrency;
        private readonly HotelSettings _hotelSettings;
        private readonly IModalService _modalService;
        private readonly INotificationService _notifications;
        private decimal _baseRateAmount;
        private string _baseRateCurrency;
        private int _maxOccupancy;

        private string _roomName = string.Empty;


        public DefineRoomTypeViewModel( CommandDispatcher commandDispatcher,
                                        IBusyIndicatorService busyIndicator,
                                        INotificationService notifications,
                                        IMessageBoxService messageBox,
                                        IModalService modalService,
                                        HotelSettings hotelSettings ):base(
            commandDispatcher,
            busyIndicator,
            notifications,
            messageBox)
        {
            _notifications = notifications;
            _modalService = modalService;
            _hotelSettings = hotelSettings;
            _baseRateCurrency = hotelSettings.DefaultCurrency;

            IsEditMode = false;
        }

        public decimal BaseRateAmount
        {
            get => _baseRateAmount;
            set
            {
                if (SetProperty(ref _baseRateAmount, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }
        public string BaseRateCurrency
        {
            get => _baseRateCurrency;
            set
            {
                if (SetProperty(ref _baseRateCurrency, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
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
                    RecalculateSaveEnabled();
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
                    RecalculateSaveEnabled();
                }
            }
        }

        private void RecalculateSaveEnabled()
        {
            IsSaveEnabled = !string.IsNullOrWhiteSpace(RoomName)
                            && !string.IsNullOrWhiteSpace(BaseRateCurrency)
                            && MaxOccupancy > 0
                            && BaseRateAmount > 0M;
        }

        protected override async Task OnSaveAsync()
        {
            DefineRoomTypeCommand cmd = new(
                RoomName,
                MaxOccupancy,
                BaseRateAmount,
                BaseRateCurrency);

            Guid roomTypeId = await ExecuteCommandAsync<DefineRoomTypeCommand, Guid>(cmd);
            if (roomTypeId == Guid.Empty) return;

            HasUnsavedChanges = false;
            _notifications.ShowSuccess($"{RoomName.Trim()} registered.");
            _modalService.Close();
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();
            return Task.CompletedTask;
        }
    }
}
