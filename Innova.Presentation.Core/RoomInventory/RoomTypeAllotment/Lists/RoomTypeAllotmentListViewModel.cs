using Innova.Application.Dispatchers;
using Innova.Application.RoomInventory.DTO;
using Innova.Application.RoomInventory.Queries;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.RoomInventory.RoomTypeAllotment.Lists
{
    /// <summary>
    ///     Availability calendar: one row per room type, one column per night in a sliding window.
    ///     Items = rows (room types), so SearchText / HasItems / IsEmpty from ListViewModel keep working.
    /// </summary>
    public sealed class RoomTypeAllotmentListViewModel:ListViewModel<RoomTypeAllotmentListItemViewModel>
    {
        private const int WeekDays = 7;
        private const int FortnightDays = 14;

        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<RoomTypeAllotmentListItemViewModel> _allRows = [];
        private IReadOnlyList<AllotmentDayViewModel> _days = [];
        private string _rangeDisplay = string.Empty;
        private int _windowDays = FortnightDays;
        private DateOnly _windowStart = DateOnly.FromDateTime(DateTime.Today);

        public RoomTypeAllotmentListViewModel( CommandDispatcher commandDispatcher,
                                               IBusyIndicatorService busyIndicator,
                                               INotificationService notifications,
                                               QueryDispatcher queryDispatcher,
                                               IModalService modalService ):base(
            commandDispatcher,
            busyIndicator,
            notifications)
        {
            _queryDispatcher = queryDispatcher;
            _modalService = modalService;

            Title = "Room Allotments";

            PreviousWindowCommand = new MvxAsyncCommand(() => ShiftWindowAsync(-1));
            NextWindowCommand = new MvxAsyncCommand(() => ShiftWindowAsync(1));
            TodayCommand = new MvxAsyncCommand(GoToTodayAsync);
            ShowWeekCommand = new MvxAsyncCommand(() => SetWindowDaysAsync(WeekDays));
            ShowFortnightCommand = new MvxAsyncCommand(() => SetWindowDaysAsync(FortnightDays));
            OpenAllotmentCommand = new MvxAsyncCommand<AllotmentCellViewModel>(OnOpenAllotmentAsync);
        }

        // ── Calendar state ───────────────────────────────────────
        public IReadOnlyList<AllotmentDayViewModel> Days
        {
            get => _days;
            private set => SetProperty(ref _days, value);
        }

        public string RangeDisplay
        {
            get => _rangeDisplay;
            private set => SetProperty(ref _rangeDisplay, value);
        }

        public int WindowDays
        {
            get => _windowDays;
            private set
            {
                if (!SetProperty(ref _windowDays, value)) return;
                RaisePropertyChanged(nameof(IsWeekWindow));
                RaisePropertyChanged(nameof(IsFortnightWindow));
            }
        }

        public bool IsWeekWindow => WindowDays == WeekDays;
        public bool IsFortnightWindow => WindowDays == FortnightDays;

        // ── Commands ─────────────────────────────────────────────
        public IMvxAsyncCommand PreviousWindowCommand { get; }
        public IMvxAsyncCommand NextWindowCommand { get; }
        public IMvxAsyncCommand TodayCommand { get; }
        public IMvxAsyncCommand ShowWeekCommand { get; }
        public IMvxAsyncCommand ShowFortnightCommand { get; }
        public IMvxAsyncCommand<AllotmentCellViewModel> OpenAllotmentCommand { get; }

        // ── Loading ──────────────────────────────────────────────
        protected override async Task LoadAsync()
        {
            // Capture once: the window must not change while the awaits below are in flight.
            DateOnly from = _windowStart;
            int dayCount = _windowDays;
            DateOnly to = from.AddDays(dayCount); // exclusive end: the window covers nights [from, to)
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            AllotmentDayViewModel[] days =
            [
                .. Enumerable
                   .Range(0, dayCount)
                   .Select(offset =>
                   {
                       DateOnly date = from.AddDays(offset);
                       return new AllotmentDayViewModel(
                           date,
                           today,
                           offset == 0 || date.Day == 1);
                   })
            ];

            // Rows come from the room types (so a type with no allotments yet still shows up),
            // cells from each type's calendar. Sequential on purpose: the room-type count is small
            // and it keeps the query scope simple.
            IReadOnlyCollection<RoomTypeDto> roomTypes = await _queryDispatcher.DispatchAsync(new ListRoomTypesQuery());

            List<RoomTypeAllotmentListItemViewModel> rows = [];
            foreach (RoomTypeDto roomType in roomTypes.OrderBy(type => type.Name))
            {
                IReadOnlyCollection<AllotmentNightDto> nights =
                    await _queryDispatcher.DispatchAsync(
                        new GetRoomTypeAllotmentCalendarQuery(roomType.RoomTypeId, from, to));

                rows.Add(
                    new RoomTypeAllotmentListItemViewModel(
                        roomType.RoomTypeId,
                        roomType.Name,
                        days,
                        nights));
            }

            Days = days;
            RangeDisplay = FormatRange(from, to.AddDays(-1));
            _allRows = rows;
            ApplyFilter();
        }

        protected override void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SetItems(_allRows);
                return;
            }

            string searchText = SearchText.Trim();

            SetItems(_allRows.Where(row => row.RoomTypeName.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
        }

        // ── Window navigation ────────────────────────────────────
        // The IsLoading guards stop a fast double-click from moving the window while a load is
        // still running (RefreshCommand ignores a second call, which would desync state and screen).
        private async Task ShiftWindowAsync( int direction )
        {
            if (IsLoading) return;

            _windowStart = _windowStart.AddDays(direction * WindowDays);
            await RefreshCommand.ExecuteAsync();
        }

        private async Task GoToTodayAsync()
        {
            if (IsLoading) return;

            _windowStart = DateOnly.FromDateTime(DateTime.Today);
            await RefreshCommand.ExecuteAsync();
        }

        private async Task SetWindowDaysAsync( int days )
        {
            if (IsLoading || days == WindowDays) return;

            WindowDays = days;
            await RefreshCommand.ExecuteAsync();
        }

        // ── Cell action ──────────────────────────────────────────
        private Task OnOpenAllotmentAsync( AllotmentCellViewModel cell ) =>
            // A cell identifies (RoomTypeId, Date). Once the details/adjust modal exists:
            //   await _modalService.ShowAsync<RoomTypeAllotmentDetailsViewModel, ...>(...);
            //   await RefreshCommand.ExecuteAsync();   // only needed if the modal can change data
            Task.CompletedTask;

        private static string FormatRange( DateOnly first, DateOnly last ) => first.Year == last.Year
                                                                                  ? $"{first:dd MMM} – {last:dd MMM yyyy}"
                                                                                  : $"{first:dd MMM yyyy} – {last:dd MMM yyyy}";
    }
}
