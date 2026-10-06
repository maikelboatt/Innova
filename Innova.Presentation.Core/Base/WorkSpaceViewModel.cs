using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;
using Serilog;

namespace Innova.Presentation.Core.Base
{
    /// <summary>
    ///     Base class for full-screen content ViewModels — the kind that
    ///     occupy the main ContentControl area of the Shell.
    ///     A Workspace is a self-contained section of the application:
    ///     GuestListViewModel, ReservationListViewModel, DashboardViewModel,
    ///     BillDetailsViewModel, etc are all Workspaces.
    ///     Adds:
    ///     - Title property for the page/screen heading
    ///     - IsLoading flag for data fetch operations distinct from
    ///     command busy state (loading a list vs submitting a form
    ///     are different UX concerns)
    ///     - LoadAsync() virtual method — called by Initialize() so
    ///     every Workspace can load its data in a consistent place
    ///     - RefreshCommand for the user to manually reload data
    /// </summary>
    public abstract class WorkspaceViewModel:ViewModelBase
    {
        private bool _isLoading;
        private string _title = string.Empty;

        protected WorkspaceViewModel( CommandDispatcher commandDispatcher, IBusyIndicatorService busyIndicator, INotificationService notifications )
            :base(commandDispatcher, busyIndicator, notifications) => RefreshCommand = new MvxAsyncCommand(OnRefreshAsync);

        // ── Title ─────────────────────────────────────────────────────
        // Bind to the page heading in the View. Each Workspace sets
        // this in its constructor or Initialize override.
        public string Title
        {
            get => _title;
            protected set => SetProperty(ref _title, value);
        }

        // ── IsLoading ─────────────────────────────────────────────────
        // Distinct from IBusyIndicatorService.IsBusy.
        // IsBusy (from BusyIndicator) covers command execution —
        // form submissions, status changes, etc.
        // IsLoading covers initial data fetch and refresh —
        // loading a guest list, fetching room details.
        // The View can show different UX for each:
        //   IsBusy → full-screen overlay with spinner
        //   IsLoading → skeleton loader or shimmer in the content area
        public bool IsLoading
        {
            get => _isLoading;
            protected set => SetProperty(ref _isLoading, value);
        }

        // ── RefreshCommand ────────────────────────────────────────────
        // Exposed to Views for a "Refresh" button or pull-to-refresh.
        // Calls LoadAsync() so subclasses only override one method.
        public IMvxAsyncCommand RefreshCommand { get; }

        // ══════════════════════════════════════════════════════════════
        // Initialize — MVVMCross calls this after the View is attached.
        // Triggers the initial data load automatically so the View
        // never shows empty content if data is available.
        // ══════════════════════════════════════════════════════════════
        public override async Task Initialize()
        {
            await base.Initialize();
            await LoadDataAsync();
        }

        // ══════════════════════════════════════════════════════════════
        // LoadDataAsync — override in subclasses to fetch initial data.
        // Wraps IsLoading so subclasses don't repeat the try/finally.
        //
        // Example override:
        //   protected override async Task LoadDataAsync()
        //   {
        //       Guests = await _queryDispatcher.QueryAsync(...);
        //   }
        // ══════════════════════════════════════════════════════════════
        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                await LoadAsync();
            }
            catch (Exception ex)
            {
                Notifications.ShowError(
                    "Failed to load data. Please refresh or contact support.");

                Log.Error(
                    ex,
                    "Failed to load data in {ViewModel}",
                    GetType()
                        .Name);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Override this in each Workspace to load initial data.
        protected virtual Task LoadAsync() => Task.CompletedTask;

        private async Task OnRefreshAsync()
        {
            await LoadDataAsync();
        }
    }
}
