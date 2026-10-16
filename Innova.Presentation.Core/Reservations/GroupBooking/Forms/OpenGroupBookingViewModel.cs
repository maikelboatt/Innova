using Innova.Application.Dispatchers;
using Innova.Application.Reservations.GroupBooking.Commands.OpenGroupBooking;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Reservations.Reservation.Forms;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Reservations.GroupBooking.Forms
{
    public sealed class OpenGroupBookingViewModel
        :FormViewModel, IMvxViewModel<Guid>
    {
        private readonly CommandDispatcher _commandDispatcher;
        private readonly IModalService _modalService;
        private string _groupName = string.Empty;

        private Guid _organizerGuestId;

        public OpenGroupBookingViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            IMessageBoxService messageBox,
            IModalService modalService )
            :base(
                commandDispatcher,
                busyIndicator,
                notifications,
                messageBox)
        {
            _commandDispatcher = commandDispatcher;
            _modalService = modalService;

            ToSingleReservationCommand = new MvxAsyncCommand(ToSingleReservationAsync);
        }

        public Guid OrganizerGuestId
        {
            get => _organizerGuestId;
            private set => SetProperty(ref _organizerGuestId, value);
        }

        public string GroupName
        {
            get => _groupName;
            set
            {
                if (SetProperty(ref _groupName, value))
                {
                    MarkAsChanged();
                    RecalculateSaveEnabled();
                }
            }
        }

        public IMvxAsyncCommand ToSingleReservationCommand { get; }


        public void Prepare( Guid parameter )
        {
            OrganizerGuestId = parameter;
        }

        private void RecalculateSaveEnabled()
        {
            IsSaveEnabled = OrganizerGuestId != Guid.Empty &&
                            !string.IsNullOrWhiteSpace(GroupName);
        }

        protected override async Task OnSaveAsync()
        {
            string groupName = GroupName.Trim();

            OpenGroupBookingCommand command = new(
                OrganizerGuestId,
                groupName);

            Guid groupBookingId = await ExecuteCommandAsync<OpenGroupBookingCommand, Guid>(new OpenGroupBookingCommand(OrganizerGuestId, GroupName));


            // The command returns the newly created GroupBooking ID.
            // Add any post-creation navigation here if required by
            // your group-booking workflow.
        }

        protected override Task OnCancelConfirmedAsync()
        {
            _modalService.Close();
            return Task.CompletedTask;

        }

        public async Task ToSingleReservationAsync()
        {
            await _modalService.ShowAsync<BookReservationViewModel, Guid>(
                OrganizerGuestId);
        }
    }
}
