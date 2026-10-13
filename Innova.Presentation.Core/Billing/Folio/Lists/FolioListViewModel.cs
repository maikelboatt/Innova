using Innova.Application.Billing.Folio.DTO;
using Innova.Application.Billing.Folio.Queries;
using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Base;
using Innova.Presentation.Core.Billing.Folio.Forms;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Billing.Folio.Lists
{
    /// <summary>
    ///     Workspace listing folios. The status chips filter on the server (a hotel accumulates a lot of settled
    ///     folios), the search box filters what was loaded.
    /// </summary>
    public sealed class FolioListViewModel:ListViewModel<FolioListItemViewModel>
    {
        private readonly IModalService _modalService;
        private readonly QueryDispatcher _queryDispatcher;

        private IReadOnlyList<FolioListItemViewModel> _allFolios = [];
        private string? _statusFilter = "Open";
        private string _summaryDisplay = string.Empty;

        public FolioListViewModel( CommandDispatcher commandDispatcher,
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

            Title = "Folios";

            StatusFilters =
            [
                new FolioStatusFilterViewModel("Open", "Open"),
                new FolioStatusFilterViewModel("Settled", "Settled"),
                new FolioStatusFilterViewModel("Void", "Void"),
                new FolioStatusFilterViewModel("All", null)
            ];
            MarkActiveFilter();

            NewFolioCommand = new MvxAsyncCommand(OnNewFolioAsync);
            OpenFolioDetailsCommand = new MvxAsyncCommand<Guid>(OnOpenFolioDetailsAsync);
            SetStatusFilterCommand = new MvxAsyncCommand<FolioStatusFilterViewModel>(SetStatusFilterAsync);
        }

        // ── Filters / summary ────────────────────────────────────
        public IReadOnlyList<FolioStatusFilterViewModel> StatusFilters { get; }

        public string SummaryDisplay
        {
            get => _summaryDisplay;
            private set => SetProperty(ref _summaryDisplay, value);
        }

        // ── Commands ─────────────────────────────────────────────
        public IMvxAsyncCommand NewFolioCommand { get; }
        public IMvxAsyncCommand<Guid> OpenFolioDetailsCommand { get; }
        public IMvxAsyncCommand<FolioStatusFilterViewModel> SetStatusFilterCommand { get; }

        // ── Loading ──────────────────────────────────────────────
        protected override async Task LoadAsync()
        {
            IReadOnlyCollection<FolioListItemDto> dtos =
                await _queryDispatcher.DispatchAsync(new ListFoliosQuery(_statusFilter));

            _allFolios = [.. dtos.Select(dto => new FolioListItemViewModel(dto))];

            ApplyFilter();
        }

        protected override void ApplyFilter()
        {
            IReadOnlyList<FolioListItemViewModel> filtered = _allFolios;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchText = SearchText.Trim();

                filtered =
                [
                    .. _allFolios.Where(folio => folio.OwnerName.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                                                 || folio.OwnerDetail.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                                                 || folio.OwnerTypeLabel.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                                                 || folio.StatusLabel.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                ];
            }

            SummaryDisplay = BuildSummary(filtered);
            SetItems(filtered);
        }

        // ── Actions ──────────────────────────────────────────────
        private async Task SetStatusFilterAsync( FolioStatusFilterViewModel option )
        {
            // RefreshCommand ignores a call made while it is running, so don't change the filter mid-load.
            if (IsLoading || option.Status == _statusFilter) return;

            _statusFilter = option.Status;
            MarkActiveFilter();

            await RefreshCommand.ExecuteAsync();
        }

        private async Task OnNewFolioAsync()
        {
            await _modalService.ShowAsync<OpenFolioViewModel, OpenFolioArgs>(new OpenFolioArgs());
            await RefreshCommand.ExecuteAsync();
        }

        private Task OnOpenFolioDetailsAsync( Guid folioId ) =>
            // Next screen: a details modal to post charges, record payments, settle and void.
            //   await _modalService.ShowAsync<FolioDetailsViewModel, Guid>(folioId);
            //   await RefreshCommand.ExecuteAsync();   // the folio's totals change from there
            Task.CompletedTask;

        protected override async Task OnItemSelectedAsync( FolioListItemViewModel item )
        {
            await base.OnItemSelectedAsync(item);
            await OnOpenFolioDetailsAsync(item.FolioId);
        }

        // ── Helpers ──────────────────────────────────────────────
        private void MarkActiveFilter()
        {
            foreach (FolioStatusFilterViewModel option in StatusFilters)
                option.IsActive = option.Status == _statusFilter;
        }

        private static string BuildSummary( IReadOnlyList<FolioListItemViewModel> folios )
        {
            if (folios.Count == 0) return string.Empty;

            string countText = folios.Count == 1
                                   ? "1 folio"
                                   : $"{folios.Count} folios";

            List<FolioListItemViewModel> outstanding = [.. folios.Where(folio => folio.HasOutstanding)];
            string[] currencies =
            [
                .. folios
                   .Select(folio => folio.Currency)
                   .Distinct()
            ];

            // Only add up money when everything is in one currency; otherwise the total would be meaningless.
            if (outstanding.Count == 0 || currencies.Length != 1) return countText;

            return $"{countText} · {currencies[0]} {outstanding.Sum(folio => folio.Balance):N2} outstanding";
        }
    }
}
