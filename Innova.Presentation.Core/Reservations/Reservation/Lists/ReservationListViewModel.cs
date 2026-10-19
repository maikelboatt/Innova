using Innova.Application.Dispatchers;
using Innova.Application.Reservations.DTO;
using Innova.Application.Reservations.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Reservations.Reservation.Details;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Reservations.Reservation.Lists
{
    public sealed class ReservationListViewModel:ListViewModel<ReservationListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<ReservationListItemViewModel> _allReservations = [];

        public ReservationListViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            QueryDispatcher queryDispatcher,
            IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Reservations";

            OpenReservationDetailsCommand = new MvxAsyncCommand<Guid>(OnOpenReservationDetailsAsync);
            OpenReservationEditCommand = new MvxAsyncCommand<Guid>(OnOpenReservationEditAsync);
        }

        public IMvxAsyncCommand<Guid> OpenReservationDetailsCommand { get; }
        public IMvxAsyncCommand<Guid> OpenReservationEditCommand { get; }

        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<ReservationDto> dtos =
                await _queryDispatcher.DispatchAsync(new GetAllReservationsQuery());

            _allReservations =
            [
                .. dtos.Select(dto =>
                                   new ReservationListItemViewModel(dto))
            ];
            ApplyFilter();
        }

        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allReservations);
                return;
            }

            string searchText = SearchText.Trim();

            IEnumerable<ReservationListItemViewModel> filtered =
                _allReservations.Where(reservation =>
                                           reservation.StayPeriodDisplay.Contains(
                                               searchText,
                                               StringComparison.OrdinalIgnoreCase)
                                           || reservation.BookingType.Contains(
                                               searchText,
                                               StringComparison.OrdinalIgnoreCase)
                                           || reservation.StatusLabel.Contains(
                                               searchText,
                                               StringComparison.OrdinalIgnoreCase)
                                           || reservation.NightlyRateDisplay.Contains(
                                               searchText,
                                               StringComparison.OrdinalIgnoreCase));

            SetItems(filtered);
        }

        private async Task OnOpenReservationDetailsAsync( Guid reservationId )
        {
            await _modalService
                .ShowAsync<ReservationDetailsViewModel, Guid>(reservationId);

            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenReservationEditAsync( Guid reservationId )
        {
            // await _modalService
            //     .ShowAsync<ReservationEditViewModel, Guid>(reservationId);

            await RefreshCommand.ExecuteAsync();
        }

        protected override async Task OnItemSelectedAsync(
            ReservationListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);

            await _modalService
                .ShowAsync<ReservationDetailsViewModel, Guid>(item.ReservationId);

            await RefreshCommand.ExecuteAsync();
        }
    }
}
