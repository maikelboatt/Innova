using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.RoomInventory.RoomType.Lists
{
    /// <summary>
    ///     Workspace showing all defined room types in a searchable,
    ///     selectable data grid. Selecting a row opens the room type
    ///     details workspace via IModalService.
    /// </summary>
    public sealed class RoomTypeListViewModel:ListViewModel<RoomTypeListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<RoomTypeListItemViewModel> _allRoomTypes = [];

        public RoomTypeListViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            QueryDispatcher queryDispatcher,
            IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Room Types";

            NewRoomTypeCommand = new MvxAsyncCommand(OnNewRoomTypeAsync);
            OpenRoomTypeDetailsCommand =
                new MvxAsyncCommand<Guid>(OnOpenRoomTypeDetailsAsync);
            OpenRoomTypeEditCommand =
                new MvxAsyncCommand<Guid>(OnOpenRoomTypeEditAsync);
        }

        // ── Commands ─────────────────────────────────────────────
        public IMvxAsyncCommand NewRoomTypeCommand { get; }
        public IMvxAsyncCommand<Guid> OpenRoomTypeDetailsCommand { get; }
        public IMvxAsyncCommand<Guid> OpenRoomTypeEditCommand { get; }

        // ── Loading ──────────────────────────────────────────────
        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<RoomTypeDto> dtos =
                await _queryDispatcher.DispatchAsync(
                    new ListRoomTypesQuery());

            _allRoomTypes =
            [
                .. dtos.Select(dto => new RoomTypeListItemViewModel(dto))
            ];
            ApplyFilter();
        }

        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allRoomTypes);
                return;
            }

            string searchText = SearchText.Trim();

            IEnumerable<RoomTypeListItemViewModel> filtered =
                _allRoomTypes.Where(roomType =>
                                        roomType.Name.Contains(
                                            searchText,
                                            StringComparison.OrdinalIgnoreCase)
                                        || roomType.OccupancyDisplay.Contains(
                                            searchText,
                                            StringComparison.OrdinalIgnoreCase)
                                        || roomType.BaseRateDisplay.Contains(
                                            searchText,
                                            StringComparison.OrdinalIgnoreCase));

            SetItems(filtered);
        }

        // ── Actions ──────────────────────────────────────────────
        private async Task OnNewRoomTypeAsync()
        {
            // await _modalService.ShowAsync<RoomTypeDefineViewModel>();
            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenRoomTypeDetailsAsync( Guid roomTypeId )
        {
            // await _modalService.ShowAsync<RoomTypeDetailsViewModel, Guid>( roomTypeId);

            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenRoomTypeEditAsync( Guid roomTypeId )
        {
            // await _modalService.ShowAsync<RoomTypeUpdateViewModel, Guid>(  roomTypeId);

            await RefreshCommand.ExecuteAsync();
        }

        protected override async Task OnItemSelectedAsync(
            RoomTypeListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);

            // await _modalService.ShowAsync<RoomTypeDetailsViewModel, Guid>(item.RoomTypeId);

            await RefreshCommand.ExecuteAsync();
        }
    }
}
