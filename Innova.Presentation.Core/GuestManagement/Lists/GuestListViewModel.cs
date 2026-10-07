using Innova.Application.Dispatchers;
using Innova.Application.GuestManagement.Guest.DTO;
using Innova.Application.GuestManagement.Guest.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.GuestManagement.Lists
{
    /// <summary>
    ///     Workspace showing all registered guests in a searchable,
    ///     selectable data grid. Selecting a row navigates to
    ///     GuestDetailsViewModel via IContentService.
    /// </summary>
    public sealed class GuestListViewModel:ListViewModel<GuestListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        // Unfiltered master list. Items (on the base class) holds
        // whatever subset SearchText currently matches; this field
        // holds everything so ApplyFilter has something to filter from.
        private IReadOnlyList<GuestListItemViewModel> _allGuests = [];

        public GuestListViewModel( CommandDispatcher commandDispatcher,
                                   IBusyIndicatorService busyIndicator,
                                   INotificationService notifications,
                                   QueryDispatcher queryDispatcher,
                                   IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Guests";

            NewGuestCommand = new MvxAsyncCommand(OnNewGuestAsync);
            OpenGuestDetailsCommand = new MvxAsyncCommand<Guid>(OnOpenGuestDetailsAsync);
            OpenGuestEditCommand = new MvxAsyncCommand<Guid>(OnOpenGuestEditAsync);
            BookReservationCommand = new MvxAsyncCommand<Guid>(OnBookReservationAsync);
        }

        // ── New commands ─────────────────────────────────────────────────
        public IMvxAsyncCommand NewGuestCommand { get; }
        public IMvxAsyncCommand<Guid> OpenGuestDetailsCommand { get; }
        public IMvxAsyncCommand<Guid> OpenGuestEditCommand { get; }
        public IMvxAsyncCommand<Guid> BookReservationCommand { get; }

        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<GuestDto> dtos = await _queryDispatcher.DispatchAsync(new GetAllGuestQuery());
            _allGuests =
            [
                .. dtos
                    .Select(dto => new GuestListItemViewModel(dto))
            ];
        }

        // ══════════════════════════════════════════════════════════
        // ApplyFilter — re-run whenever SearchText changes (base
        // class calls this automatically). Filters the in-memory
        // master list; no round-trip to the database on every
        // keystroke.
        // ══════════════════════════════════════════════════════════
        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allGuests);
                return;
            }

            IEnumerable<GuestListItemViewModel> filtered = _allGuests.Where(p => p.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                                                 || p.PhoneNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                                                                 || (p.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ??
                                                                                     false));

            SetItems(filtered);
        }

        private async Task OnNewGuestAsync()
        {
            // await _modalService.ShowAsync<GuestRegisterViewModel>();
            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenGuestDetailsAsync( Guid guestId )
        {
            // await _modalService.ShowAsync<GuestDetailsViewModel,guestId>();
            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnOpenGuestEditAsync( Guid guestId )
        {
            // await  _modalService.ShowAsync<GuestUpdateViewModel,guestId>();
            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnBookReservationAsync( Guid guestId )
        {
            // await  _modalService.ShowAsync<ReservationBookViewModel,guestId>();
        }

        // ══════════════════════════════════════════════════════════
        // OnItemSelected — navigate to the guest's detail workspace.
        // ══════════════════════════════════════════════════════════
        protected override async Task OnItemSelectedAsync( GuestListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);
            // _modalService.ShowAsync<GuestDetailsViewModel, Guid>(item.GuestId);

            await RefreshCommand.ExecuteAsync(); // Refresh the list after returning from details
        }
    }
}
