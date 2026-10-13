using System.Collections.ObjectModel;
using Innova.Application.Dispatchers;
using Innova.Application.Reservations.DTO;
using Innova.Application.Reservations.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Reservations.Reservation.Details;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Reservations.GroupBooking.Details
{
    public sealed class GroupBookingReservationsViewModel
        :DetailsViewModel<Guid>
    {
        private readonly QueryDispatcher _queryDispatcher;

        private string _groupName = string.Empty;
        private ReservationDto? _selectedReservation;

        public GroupBookingReservationsViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            IModalService modalService,
            QueryDispatcher queryDispatcher )
            :base(
                commandDispatcher,
                busyIndicator,
                notifications,
                modalService)
        {
            _queryDispatcher = queryDispatcher;

            Reservations = [];

            OpenReservationCommand =
                new MvxAsyncCommand(
                    OnOpenReservation,
                    () => SelectedReservation is not null);

            // Add attach/detach commands when their handlers
            // and the available-reservations selector are wired in.
        }

        public string GroupName
        {
            get => _groupName;
            private set => SetProperty(ref _groupName, value);
        }

        public ObservableCollection<ReservationDto> Reservations { get; }

        public ReservationDto? SelectedReservation
        {
            get => _selectedReservation;
            set
            {
                if (SetProperty(ref _selectedReservation, value))
                {
                    OpenReservationCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public IMvxAsyncCommand OpenReservationCommand { get; }

        private async Task OnOpenReservation()
        {
            if (SelectedReservation is null)
                return;

            await ModalService.NavigateAsync<
                ReservationDetailsViewModel,
                Guid>(SelectedReservation.ReservationId);
        }

        protected override async Task LoadAsync()
        {
            GroupBookingDto groupBooking =
                await _queryDispatcher.DispatchAsync(
                    new GetGroupBookingByIdQuery(Key));

            GroupName = groupBooking.GroupName;

            Reservations.Clear();

            foreach (Guid reservationId in groupBooking.ReservationIds)
            {
                ReservationDto reservation =
                    await _queryDispatcher.DispatchAsync(
                        new GetReservationByIdQuery(reservationId));

                Reservations.Add(reservation);
            }

            SelectedReservation = null;
        }
    }
}
