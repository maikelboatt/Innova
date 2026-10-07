
using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Serilog;

namespace Innova.Presentation.Core.Base
{
    /// <summary>
    ///     Base class for read-only "details" ViewModels shown as a
    ///     modal via IModalService. Examples: PatientDetailsViewModel,
    ///     AppointmentDetailsViewModel, ConsultationDetailsViewModel,
    ///     PrescriptionDetailsViewModel, BillDetailsViewModel.
    ///     Deliberately NOT WorkspaceViewModel — Details never occupies
    ///     the Shell's main content area, and "Refresh" doesn't map
    ///     cleanly onto a transient modal the same way it does a
    ///     persistent list. NOT FormViewModel — there's nothing to save
    ///     here; editing is delegated to a separate Form ViewModel via
    ///     IModalService.NavigateAsync. NOT DialogViewModel — that class
    ///     assumes one long-lived, reused instance toggled via IsVisible
    ///     with its own ShowAsync(title, message); Details ViewModels
    ///     are resolved fresh per record through
    ///     ModalService.ShowAsync&lt;TViewModel,TKey&gt;.
    ///     Adds on top of ViewModelBase:
    ///     - TKey Key, populated via Prepare() before Initialize() runs
    ///     - IsLoading, wrapping LoadAsync() in the same
    ///     try/catch/finally shape WorkspaceViewModel.LoadDataAsync
    ///     uses, so every Details ViewModel gets consistent
    ///     load-failure handling without inheriting Workspace-only
    ///     concepts (Title, RefreshCommand) that don't apply here
    ///     - CloseCommand, wired to IModalService.Close()
    /// </summary>
    public abstract class DetailsViewModel<TKey>:ViewModelBase, IMvxViewModel<TKey>
        where TKey : notnull
    {
        private bool _isLoading;

        protected DetailsViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications,
            IModalService modalService )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            ModalService = modalService;
            CloseCommand = new MvxCommand(() => ModalService.Close());
        }

        // Exposed to subclasses so they can call ModalService.NavigateAsync
        // for their own actions (e.g. Edit) without each one taking a
        // second constructor dependency on IModalService.
        protected IModalService ModalService { get; }

        // The record identifier, set via Prepare() before Initialize().
        // "= default!" is safe here: TKey is always set by Prepare()
        // before LoadAsync ever reads it, since ModalService.ShowAsync
        // calls Prepare then Initialize in that order.
        protected TKey Key { get; private set; } = default!;

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public IMvxCommand CloseCommand { get; }

        public void Prepare( TKey parameter ) => Key = parameter;

        public override async Task Initialize()
        {
            await base.Initialize();
            await LoadDetailsAsync();
        }

        private async Task LoadDetailsAsync()
        {
            try
            {
                IsLoading = true;
                await LoadAsync();
            }
            catch (Exception ex)
            {
                Notifications.ShowError("Failed to load details. Please close and try again.");
                Log.Error(
                    ex,
                    "Failed to load details in {ViewModel} for key {Key}",
                    GetType()
                        .Name,
                    Key);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected abstract Task LoadAsync();
    }
}