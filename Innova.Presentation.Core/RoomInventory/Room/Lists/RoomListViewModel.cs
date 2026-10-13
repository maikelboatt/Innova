using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.RoomInventory.Room.Lists
{
    /// <summary>
    ///     Workspace showing all rooms in the property in a searchable,
    ///     selectable data grid. Selecting a row opens the room details
    ///     workspace via IModalService.
    /// </summary>
    public sealed class RoomListViewModel:ListViewModel<RoomListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<RoomListItemViewModel> _allRooms = [];

        public RoomListViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            QueryDispatcher queryDispatcher,
            IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Rooms";

            NewRoomCommand = new MvxAsyncCommand(OnNewRoomAsync);
            OpenRoomDetailsCommand = new MvxAsyncCommand<Guid>(OnOpenRoomDetailsAsync);
            OpenRoomEditCommand = new MvxAsyncCommand<Guid>(OnOpenRoomEditAsync);
        }

        // ── Commands ──────────────────────────────────────────────
        public IMvxAsyncCommand NewRoomCommand { get; }
        public IMvxAsyncCommand<Guid> OpenRoomDetailsCommand { get; }
        public IMvxAsyncCommand<Guid> OpenRoomEditCommand { get; }

        // ── Loading ───────────────────────────────────────────────
        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<RoomDto> dtos =
                await _queryDispatcher.DispatchAsync(new GetAllRoomsQuery());

            _allRooms =
            [
                .. dtos.Select(dto => new RoomListItemViewModel(dto))
            ];
            ApplyFilter();
        }


        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allRooms);
                return;
            }

            string searchText = SearchText.Trim();

            IEnumerable<RoomListItemViewModel> filtered =
                _allRooms.Where(room =>
                                    room.RoomNumber.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase)
                                    || room.FloorDisplay.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase)
                                    || (room.FloorWing?.Contains(
                                            searchText,
                                            StringComparison.OrdinalIgnoreCase) ?? false)
                                    || room.RoomTypeName.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase)
                                    || room.StatusLabel.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase));

            SetItems(filtered);
        }

        // ── Actions ──────────────────────────────────────────────
        private async Task OnNewRoomAsync()
        {
            // await _modalService.ShowAsync<RoomRegisterViewModel>();
            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenRoomDetailsAsync( Guid roomId )
        {
            // await _modalService.ShowAsync<RoomDetailsViewModel, Guid>(roomId);
            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenRoomEditAsync( Guid roomId )
        {
            // await _modalService.ShowAsync<RoomUpdateViewModel, Guid>(roomId);
            await RefreshCommand.ExecuteAsync();
        }

        // ── Selection ────────────────────────────────────────────
        /// <summary>
        ///     Opens the selected room's detail workspace.
        /// </summary>
        protected override async Task OnItemSelectedAsync(
            RoomListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);

            // await _modalService.ShowAsync<RoomDetailsViewModel, Guid>(
            //     item.RoomId);

            await RefreshCommand.ExecuteAsync();
        }
    }
}
