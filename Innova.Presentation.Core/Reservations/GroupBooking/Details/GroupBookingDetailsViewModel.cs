using Innova.Application.Dispatchers;
using Innova.Application.Reservations.DTO;
using Innova.Application.Reservations.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Reservations.GroupBooking.Details
{
    public sealed class GroupBookingDetailsViewModel:DetailsViewModel<Guid>
    {
        private readonly QueryDispatcher _queryDispatcher;
        private string _groupName = string.Empty;

        private Guid _organizerGuestId;
        private string _organizerName = string.Empty;
        private int _reservationCount;

        public GroupBookingDetailsViewModel( CommandDispatcher commandDispatcher,
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

            OpenAssociatedReservationsCommand = new MvxAsyncCommand(OnOpenAssociatedReservations);
            OpenOrganizerCommand = new MvxAsyncCommand(OnOpenOrganizer);
            OpenGroupBookingEditCommand = new MvxAsyncCommand(OnOpenGroupBookingEdit);
        }


        public Guid OrganizerGuestId
        {
            get => _organizerGuestId;
            private set => SetProperty(ref _organizerGuestId, value);
        }

        public string OrganizerName
        {
            get => _organizerName;
            private set => SetProperty(ref _organizerName, value);
        }

        public string GroupName
        {
            get => _groupName;
            private set => SetProperty(ref _groupName, value);
        }

        public int ReservationCount
        {
            get => _reservationCount;
            private set => SetProperty(ref _reservationCount, value);
        }


        public IMvxAsyncCommand OpenAssociatedReservationsCommand { get; }

        public IMvxAsyncCommand OpenOrganizerCommand { get; }

        public IMvxAsyncCommand OpenGroupBookingEditCommand { get; }


        protected override async Task LoadAsync()
        {
            GroupBookingDto dto = await _queryDispatcher.DispatchAsync(new GetGroupBookingByIdQuery(Key));

            OrganizerGuestId = dto.OrganizerGuestId;
            OrganizerName = dto.OrganizerName;
            GroupName = dto.GroupName;
            ReservationCount = dto.ReservationIds.Count;
        }


        private async Task OnOpenAssociatedReservations()
        {
            await ModalService.NavigateAsync<GroupBookingReservationsViewModel, Guid>(Key);
        }

        private async Task OnOpenOrganizer()
        {
            // Navigate to your GuestDetailsViewModel when implemented.
            await Task.CompletedTask;
        }

        private async Task OnOpenGroupBookingEdit()
        {
            // Navigate to GroupBookingEditViewModel when implemented.
            await Task.CompletedTask;
        }
    }
}
