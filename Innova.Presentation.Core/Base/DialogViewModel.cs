using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Base
{
    /// <summary>
    ///     Base class for ViewModels that represent modal dialogs.
    ///     Examples: ConfirmationDialogViewModel, ErrorDialogViewModel
    ///     and any future dialog (e.g. CancelReservationDialog,
    ///     DetachFromGroupBookingDialog that need a reason input)
    ///     Dialogs are different from Workspaces and Forms in two key ways:
    ///     1. They have a Result — confirmed/cancelled, true/false
    ///     2. They do not navigate the content area — they float above it
    ///     Adds on top of ViewModelBase:
    ///     - ConfirmCommand and DismissCommand
    ///     - TaskCompletionSource pattern so callers can await the result:
    ///     bool confirmed = await dialog.ShowAsync();
    ///     - Title and Message properties for the dialog content
    /// </summary>
    public abstract class DialogViewModel:ViewModelBase
    {
        private bool _isVisible;
        private string _message = string.Empty;

        // TCS lets the caller await the dialog result
        // without needing callbacks or events
        private TaskCompletionSource<bool> _resultSource = new();
        private string _title = string.Empty;

        protected DialogViewModel(
            CommandDispatcher commandDispatcher,
            IBusyIndicatorService busyIndicator,
            INotificationService notifications )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            ConfirmCommand = new MvxCommand(OnConfirm);
            DismissCommand = new MvxCommand(OnDismiss);
        }

        // ── Display Properties ────────────────────────────────────────
        public string Title
        {
            get => _title;
            protected set => SetProperty(ref _title, value);
        }

        public string Message
        {
            get => _message;
            protected set => SetProperty(ref _message, value);
        }

        // ── IsVisible ─────────────────────────────────────────────────
        // Bind to the dialog overlay's Visibility.
        // The dialog shows/hides itself via ShowAsync/Close
        // rather than being created and destroyed each time.
        public bool IsVisible
        {
            get => _isVisible;
            private set => SetProperty(ref _isVisible, value);
        }

        // ── Commands ──────────────────────────────────────────────────
        public IMvxCommand ConfirmCommand { get; }
        public IMvxCommand DismissCommand { get; }

        // ══════════════════════════════════════════════════════════════
        // ShowAsync — display the dialog and await the user's choice.
        //
        // Usage from a FormViewModel:
        //   bool confirmed = await _confirmDialog.ShowAsync(
        //       "Delete Patient",
        //       "Are you sure you want to remove this patient?");
        //
        //   if (confirmed) { ... proceed ... }
        // ══════════════════════════════════════════════════════════════
        public Task<bool> ShowAsync( string title, string message )
        {
            Title = title;
            Message = message;

            // Reset the result source for reuse
            _resultSource = new TaskCompletionSource<bool>();

            IsVisible = true;

            return _resultSource.Task;
        }

        // ══════════════════════════════════════════════════════════════
        // Close — hides the dialog and completes the awaited result.
        // Called by OnConfirm and OnDismiss.
        // ══════════════════════════════════════════════════════════════
        protected void Close( bool result )
        {
            IsVisible = false;
            _resultSource.TrySetResult(result);
        }

        // ── Default implementations — override for custom behaviour ───
        protected virtual void OnConfirm() => Close(true);

        protected virtual void OnDismiss() => Close(false);
    }
}
