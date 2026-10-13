using Innova.Application.Dispatchers;
using Innova.Application.Reservations.DTO;
using Innova.Application.Reservations.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Reservations.GroupBooking.Lists
{
    public sealed class GroupBookingListViewModel
        :ListViewModel<GroupBookingListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<GroupBookingListItemViewModel> _allGroupBookings = [];

        public GroupBookingListViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            QueryDispatcher queryDispatcher,
            IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Group Bookings";

            NewGroupBookingCommand = new MvxAsyncCommand(OnNewGroupBookingAsync);
            OpenGroupBookingDetailsCommand = new MvxAsyncCommand<Guid>(OnOpenGroupBookingDetailsAsync);
            OpenGroupBookingEditCommand = new MvxAsyncCommand<Guid>(OnOpenGroupBookingEditAsync);
        }

        public IMvxAsyncCommand NewGroupBookingCommand { get; }

        public IMvxAsyncCommand<Guid> OpenGroupBookingDetailsCommand { get; }

        public IMvxAsyncCommand<Guid> OpenGroupBookingEditCommand { get; }

        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<GroupBookingDto> dtos =
                await _queryDispatcher.DispatchAsync(
                    new GetAllGroupBookingsQuery());

            _allGroupBookings =
            [
                .. dtos.Select(dto =>
                                   new GroupBookingListItemViewModel(dto))
            ];

            ApplyFilter();
        }

        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allGroupBookings);
                return;
            }

            string searchText = SearchText.Trim();

            IEnumerable<GroupBookingListItemViewModel> filtered =
                _allGroupBookings.Where(groupBooking =>
                                            groupBooking.GroupName.Contains(
                                                searchText,
                                                StringComparison.OrdinalIgnoreCase)
                                            || groupBooking.ReservationCountDisplay.Contains(
                                                searchText,
                                                StringComparison.OrdinalIgnoreCase));

            SetItems(filtered);
        }

        private async Task OnNewGroupBookingAsync()
        {
            // await _modalService.ShowAsync<GroupBookingCreateViewModel>();
            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenGroupBookingDetailsAsync(
            Guid groupBookingId )
        {
            // await _modalService
            //     .ShowAsync<GroupBookingDetailsViewModel, Guid>(
            //         groupBookingId);

            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenGroupBookingEditAsync(
            Guid groupBookingId )
        {
            // await _modalService
            //     .ShowAsync<GroupBookingEditViewModel, Guid>(
            //         groupBookingId);

            await RefreshCommand.ExecuteAsync();
        }

        protected override async Task OnItemSelectedAsync(
            GroupBookingListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);

            // await _modalService
            //     .ShowAsync<GroupBookingDetailsViewModel, Guid>(
            //         item.GroupBookingId);

            await RefreshCommand.ExecuteAsync();
        }
    }
}
