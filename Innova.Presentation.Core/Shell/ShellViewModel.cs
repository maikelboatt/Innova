using System.Collections.ObjectModel;
using Innova.Application.Abstractions.Services;
using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Billing.Folio.Lists;
using Innova.Presentation.Core.Dashboard;
using Innova.Presentation.Core.FrontDesk.Stay.Lists;
using Innova.Presentation.Core.GuestManagement.Lists;
using Innova.Presentation.Core.HouseKeeping.HouseKeeping.Lists;
using Innova.Presentation.Core.Reservations.GroupBooking.Lists;
using Innova.Presentation.Core.Reservations.Reservation.Lists;
using Innova.Presentation.Core.RoomInventory.Room.Lists;
using Innova.Presentation.Core.RoomInventory.RoomType.Lists;
using Innova.Presentation.Core.RoomInventory.RoomTypeAllotment.Lists;
using Innova.Presentation.Core.Services;
using Innova.Presentation.Core.Services.Abstractions;
using Innova.Presentation.Core.Settings;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Innova.Presentation.Core.Shell
{
    public class ShellViewModel:ViewModelBase
    {
        private readonly IContentService _contentService;
        private readonly ICurrentUserContext _currentUserContext;
        private readonly IModalService _modalService;
        private readonly IMvxNavigationService _mvxNavigationService;
        private readonly INotificationFeed _notificationFeed;

        private string _activeNavItem = "Dashboard";
        private string _breadcrumb = "Home / Dashboard";

        private MvxViewModel? _currentMainContent;
        private bool _isNavExpanded = true;

        public ShellViewModel( CommandDispatcher commandDispatcher,
                               IBusyIndicatorService busyIndicator,
                               INotificationService notifications,
                               INotificationFeed notificationFeed,
                               IContentService contentService,
                               IModalService modalService,
                               ICurrentUserContext currentUserContext,
                               IMvxNavigationService mvxNavigationService ):base(
            commandDispatcher,
            busyIndicator,
            notifications)
        {
            _notificationFeed = notificationFeed;
            _contentService = contentService;
            _modalService = modalService;
            _currentUserContext = currentUserContext;
            _mvxNavigationService = mvxNavigationService;

            // ── Navigation commands ────────────────────────────────
            NavigateToDashboardCommand = new MvxCommand(() => Navigate<DashboardViewModel>("Dashboard", "Home / Dashboard"));
            NavigateToGuestsCommand = new MvxCommand(() => Navigate<GuestListViewModel>("Guests", "Home / Guests"));
            NavigateToRoomsCommand = new MvxCommand(() => Navigate<RoomListViewModel>("Rooms", "Home / Room Inventory"));
            NavigateToRoomDefinitionsCommand = new MvxCommand(() => Navigate<RoomTypeListViewModel>("Room Type", "Home / Room Inventory"));
            NavigateToRoomAllotmentsCommand = new MvxCommand(() => Navigate<RoomTypeAllotmentListViewModel>("Room Allotment", "Home / Room Inventory"));
            NavigateToSingleReservationsCommand =
                new MvxCommand(() => Navigate<ReservationListViewModel>("Individual Reservation", "Home / Reservations"));
            NavigateToGroupBookingsCommand = new MvxCommand(() => Navigate<GroupBookingListViewModel>("Group Booking", "Home / Reservations"));
            NavigateToStayCommand = new MvxCommand(() => Navigate<StayListViewModel>("Stay", "Home / Stay"));
            NavigateToHouseKeepingCommand = new MvxCommand(() => Navigate<HouseKeepingTaskListViewModel>("House Keeping", "Home / House Keeping"));
            NavigateToBillingCommand = new MvxCommand(() => Navigate<FolioListViewModel>("Billing", "Home / Billing"));
            NavigateToSettingsCommand = new MvxCommand(() => Navigate<SettingsViewModel>("Settings", "Home / Settings"));

            // ── Window chrome commands ─────────────────────────────

            _contentService.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modalService.ModalChanged += OnModalChanged;
            _currentUserContext.SignedOut += OnSignedOut;
        }

        // Main content - bound to the ContentControl in the shell
        // ══════════════════════════════════════════════════════════════
        public MvxViewModel? CurrentMainContent
        {
            get => _currentMainContent;
            private set => SetProperty(ref _currentMainContent, value);
        }

        // Modal - driven by IModalService
        // ══════════════════════════════════════════════════════════════
        public bool IsModalOpen => _modalService.CurrentModalViewModel is not null;
        public MvxViewModel? CurrentModalContent => _modalService.CurrentModalViewModel;

        // Nav State
        // ══════════════════════════════════════════════════════════════
        public string ActiveNavItem
        {
            get => _activeNavItem;
            private set => SetProperty(ref _activeNavItem, value);
        }

        public string Breadcrumb
        {
            get => _breadcrumb;
            private set => SetProperty(ref _breadcrumb, value);
        }


        // ── IsActive helpers — bind button active style ────────────
        // ══════════════════════════════════════════════════════════════
        public bool IsDashboardActive => ActiveNavItem == "Dashboard";
        public bool IsGuestsActive => ActiveNavItem == "Guests";
        public bool IsRoomsActive => ActiveNavItem == "Rooms";
        public bool IsRoomTypeActive => ActiveNavItem == "Room Type";
        public bool IsRoomAllotmentActive => ActiveNavItem == "Room Allotment";
        public bool IsReservationsActive => ActiveNavItem == "Reservations";
        public bool IsGroupBookingActive => ActiveNavItem == "GroupBookings";
        public bool IsStayActive => ActiveNavItem == "Stay";
        public bool IsHouseKeepingActive => ActiveNavItem == "House Keeping";
        public bool IsBillingActive => ActiveNavItem == "Billing";
        public bool IsSettingsActive => ActiveNavItem == "Settings";


        // - Notification feed
        // ══════════════════════════════════════════════════════════════
        public ObservableCollection<NotificationItem> Toasts => _notificationFeed.Notifications;

        // Navigation commands
        // ══════════════════════════════════════════════════════════════
        public IMvxCommand NavigateToDashboardCommand { get; }
        public IMvxCommand NavigateToGuestsCommand { get; }
        public IMvxCommand NavigateToRoomsCommand { get; }
        public IMvxCommand NavigateToRoomDefinitionsCommand { get; }
        public IMvxCommand NavigateToRoomAllotmentsCommand { get; }
        public IMvxCommand NavigateToSingleReservationsCommand { get; }
        public IMvxCommand NavigateToGroupBookingsCommand { get; }
        public IMvxCommand NavigateToStayCommand { get; }
        public IMvxCommand NavigateToHouseKeepingCommand { get; }
        public IMvxCommand NavigateToBillingCommand { get; }
        public IMvxCommand NavigateToSettingsCommand { get; }
        public IMvxCommand ToggleNavCommand { get; }

        public override Task Initialize()
        {
            _contentService.NavigateTo<DashboardViewModel>();
            return base.Initialize();
        }

        // Navigate - delegates to IContentService and updates nav state
        // ══════════════════════════════════════════════════════════════
        private void Navigate<TViewModel>( string navItem, string breadcrumb ) where TViewModel : MvxViewModel
        {
            _contentService.NavigateTo<TViewModel>();
            ActiveNavItem = navItem;
            Breadcrumb = breadcrumb;

            RaiseAllIsActiveProperties();
        }

        // Event handlers
        // ══════════════════════════════════════════════════════════════
        private void OnCurrentViewModelChanged( object? sender, MvxViewModel vm )
        {
            CurrentMainContent = vm;
        }

        private void OnModalChanged()
        {
            RaisePropertyChanged(nameof(IsModalOpen));
            RaisePropertyChanged(nameof(CurrentModalContent));
        }

        private async void OnSignedOut()
        {
            // await _mvxNavigationService.Navigate<LoginViewModel>();
        }

        private void RaiseAllIsActiveProperties()
        {
            RaisePropertyChanged(nameof(IsDashboardActive));
            RaisePropertyChanged(nameof(IsGuestsActive));
            RaisePropertyChanged(nameof(IsRoomsActive));
            RaisePropertyChanged(nameof(IsRoomTypeActive));
            RaisePropertyChanged(nameof(IsRoomAllotmentActive));
            RaisePropertyChanged(nameof(IsReservationsActive));
            RaisePropertyChanged(nameof(IsGroupBookingActive));
            RaisePropertyChanged(nameof(IsStayActive));
            RaisePropertyChanged(nameof(IsHouseKeepingActive));
            RaisePropertyChanged(nameof(IsBillingActive));
            RaisePropertyChanged(nameof(IsSettingsActive));
        }

        // Cleanup — unsubscribe from modal service event
        // ══════════════════════════════════════════════════════════════
        public override void ViewDestroy( bool viewFinishing = true )
        {
            _contentService.CurrentViewModelChanged -= OnCurrentViewModelChanged;
            _modalService.ModalChanged -= OnModalChanged;
            _currentUserContext.SignedOut -= OnSignedOut;
            base.ViewDestroy(viewFinishing);
        }
    }
}
