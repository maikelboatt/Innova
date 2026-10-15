using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.RoomInventory.RoomType.Forms;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.RoomInventory.RoomType.Details
{
    public sealed class RoomTypeDetailsViewModel:DetailsViewModel<Guid>
    {
        private readonly IMessageBoxService _messageBoxService;
        private readonly QueryDispatcher _queryDispatcher;
        private decimal _baseRateAmount;
        private string _baseRateCurrency;
        private int _maxOccupancy;
        private string _name;


        public RoomTypeDetailsViewModel( CommandDispatcher commandDispatcher,
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

            OpenRoomTypeEditCommand = new MvxAsyncCommand(OnOpenRoomTypeEdit);
        }

        public decimal BaseRateAmount
        {
            get => _baseRateAmount;
            set => SetProperty(ref _baseRateAmount, value);
        }
        public string BaseRateCurrency
        {
            get => _baseRateCurrency;
            set => SetProperty(ref _baseRateCurrency, value);
        }
        public int MaxOccupancy
        {
            get => _maxOccupancy;
            set => SetProperty(ref _maxOccupancy, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public IMvxAsyncCommand OpenRoomTypeEditCommand { get; }

        private async Task OnOpenRoomTypeEdit()
        {
            await ModalService.NavigateAsync<UpdateRoomTypeViewModel, Guid>(Key);
        }

        protected override async Task LoadAsync()
        {
            RoomTypeDto dto = await _queryDispatcher.DispatchAsync(new GetRoomTypeByIdQuery(Key));

            Name = dto.Name;
            MaxOccupancy = dto.MaxOccupancy;
            BaseRateAmount = dto.BaseRateAmount;
            BaseRateCurrency = dto.BaseRateCurrency;
        }
    }
}
