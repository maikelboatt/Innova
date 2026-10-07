using Innova.Application.Dispatchers;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Commands;

namespace Innova.Presentation.Core.Base
{
    /// <summary>
    ///     Base class for ViewModels that represent a create or edit form.
    ///     Examples: RegisterGuestViewModel, BookReservationViewModel,
    ///     CheckInRoomViewModel, AssignHouseKeepingTaskViewModel
    ///     Adds on top of ViewModelBase:
    ///     - SaveCommand and CancelCommand wired to abstract methods
    ///     - IsEditMode flag to distinguish Create vs Edit
    ///     - HasUnsavedChanges for unsaved change detection
    ///     - IsSaveEnabled for disabling Save until form is valid
    /// </summary>
    public abstract class FormViewModel:ViewModelBase
    {
        private bool _hasUnsavedChanges;
        private bool _isEditMode;
        private bool _isSaveEnabled = true;

        protected FormViewModel( CommandDispatcher commandDispatcher,
                                 IBusyIndicatorService busyIndicator,
                                 INotificationService notifications,
                                 IMessageBoxService messageBox )
            :base(commandDispatcher, busyIndicator, notifications)
        {
            MessageBox = messageBox;

            SaveCommand = new MvxAsyncCommand(OnSaveAsync, () => IsSaveEnabled);

            CancelCommand = new MvxAsyncCommand(OnCancelAsync);
        }

        protected IMessageBoxService MessageBox { get; }

        // ── IsEditMode ────────────────────────────────────────────────
        // False = creating a new record (Register Patient, Book Appointment)
        // True  = editing an existing record (Edit Patient, Reschedule)
        // Views can bind to this to change labels ("Register" vs "Save Changes")
        public bool IsEditMode
        {
            get => _isEditMode;
            protected set => SetProperty(ref _isEditMode, value);
        }

        // ── HasUnsavedChanges ─────────────────────────────────────────
        // Set to true whenever any form field changes.
        // Used by OnCancelAsync to warn the user before discarding edits.
        public bool HasUnsavedChanges
        {
            get => _hasUnsavedChanges;
            protected set => SetProperty(ref _hasUnsavedChanges, value);
        }

        // ── IsSaveEnabled ─────────────────────────────────────────────
        // Controls whether SaveCommand can execute.
        // Bind to Save button's IsEnabled or opacity.
        // Subclasses set this false until required fields are filled.
        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            protected set
            {
                if (SetProperty(ref _isSaveEnabled, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        // ── Commands ──────────────────────────────────────────────────
        public IMvxAsyncCommand SaveCommand { get; }
        public IMvxAsyncCommand CancelCommand { get; }

        // ══════════════════════════════════════════════════════════════
        // OnSaveAsync — override to build and dispatch the command.
        //
        // Example:
        //   protected override async Task OnSaveAsync()
        //   {
        //       var command = new RegisterGuestCommand(...);
        //       Guid id = await ExecuteCommandAsync
        //           RegisterGuestCommand, Guid>(command);
        //       if (id != default)
        //       {
        //           HasUnsavedChanges = false;
        //           _contentService.NavigateTo<GuestDetailsViewModel>(id);
        //       }
        //   }
        // ══════════════════════════════════════════════════════════════
        protected abstract Task OnSaveAsync();

        // ══════════════════════════════════════════════════════════════
        // OnCancelAsync — default behaviour warns if unsaved changes exist.
        // Override to add navigation (go back to list, close dialog, etc.)
        // ══════════════════════════════════════════════════════════════
        protected virtual async Task OnCancelAsync()
        {
            if (HasUnsavedChanges)
            {
                bool confirmed = await MessageBox.ShowConfirmAsync(
                                     "Discard changes?",
                                     "You have unsaved changes. Are you sure you want to leave?");

                if (!confirmed)
                    return;
            }

            HasUnsavedChanges = false;
            await OnCancelConfirmedAsync();
        }

        // Override this in subclasses to handle confirmed cancellation:
        // navigate back to list, close dialog, switch content area, etc.
        protected virtual Task OnCancelConfirmedAsync() => Task.CompletedTask;

        // ══════════════════════════════════════════════════════════════
        // MarkAsChanged — call this from any property setter to flag
        // that the form has unsaved changes.
        //
        // Usage in a subclass property:
        //   set
        //   {
        //       if (SetProperty(ref _firstName, value))
        //           MarkAsChanged();
        //   }
        // ══════════════════════════════════════════════════════════════
        protected void MarkAsChanged()
        {
            HasUnsavedChanges = true;
        }
    }
}
