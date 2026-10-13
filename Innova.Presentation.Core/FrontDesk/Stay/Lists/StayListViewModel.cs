using Innova.Application.Dispatchers;
using Innova.Application.FrontDesk.Stay.DTO;
using Innova.Application.FrontDesk.Stay.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.FrontDesk.Stay.Lists
{
    public sealed class StayListViewModel:ListViewModel<StayListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<StayListItemViewModel> _allStays = [];

        public StayListViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            QueryDispatcher queryDispatcher,
            IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Current Stays";

            OpenStayDetailsCommand =
                new MvxAsyncCommand<Guid>(OnOpenStayDetailsAsync);

            OpenAddOccupantCommand =
                new MvxAsyncCommand<Guid>(OnAddOccupantAsync);
        }

        public IMvxAsyncCommand<Guid> OpenStayDetailsCommand { get; }
        public IMvxAsyncCommand<Guid> OpenAddOccupantCommand { get; }

        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<StaySummaryDto> dtos =
                await _queryDispatcher.DispatchAsync(
                    new ListCurrentStaysQuery());

            _allStays =
            [
                .. dtos.Select(dto =>
                                   new StayListItemViewModel(dto))
            ];
            ApplyFilter();
        }

        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allStays);
                return;
            }

            string searchText = SearchText.Trim();

            IEnumerable<StayListItemViewModel> filtered =
                _allStays.Where(stay =>
                                    stay.RoomNumber.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase)
                                    || stay.ActualCheckInDisplay.Contains(
                                        searchText,
                                        StringComparison.OrdinalIgnoreCase));

            SetItems(filtered);
        }

        private async Task OnOpenStayDetailsAsync( Guid stayId )
        {
            // await _modalService
            //     .ShowAsync<StayDetailsViewModel, Guid>(stayId);

            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnAddOccupantAsync( Guid stayId )
        {
            // await _modalService
            //     .ShowAsync<AddOccupantViewModel, Guid>(stayId);

            await RefreshCommand.ExecuteAsync();
        }

        protected override async Task OnItemSelectedAsync(
            StayListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);

            // await _modalService
            //     .ShowAsync<StayDetailsViewModel, Guid>(
            //         item.StayId);

            await RefreshCommand.ExecuteAsync();
        }
    }
}
