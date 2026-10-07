using Innova.Application.Abstractions.Messaging;
using Innova.Application.Dispatchers;
using Innova.Application.Exceptions;
using Innova.Domain.Shared.Exceptions;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.ViewModels;
using Serilog;

namespace Innova.Presentation.Core.Base
{
    /// <summary>
    ///     Root base class for all ViewModels in Innova.
    ///     Responsibilities:
    ///     - Command dispatch via CommandDispatcher
    ///     - Busy state management via IBusyIndicatorService
    ///     - Error/notification feedback via INotificationService
    ///     - Centralised exception handling for all command executions
    ///     All ViewModels inherit from this either directly or through
    ///     one of the specialised base classes below it.
    /// </summary>
    public abstract class ViewModelBase:MvxViewModel
    {
        protected readonly IBusyIndicatorService BusyIndicator;
        protected readonly INotificationService Notifications;
        private readonly CommandDispatcher _commandDispatcher;

        protected ViewModelBase( CommandDispatcher commandDispatcher, IBusyIndicatorService busyIndicator, INotificationService notifications )
        {
            _commandDispatcher = commandDispatcher;
            BusyIndicator = busyIndicator;
            Notifications = notifications;
        }

        // ══════════════════════════════════════════════════════════════
        // ExecuteCommandAsync<TCommand, TResult>
        //
        // The single entry point for all command dispatch from ViewModels.
        // Every button click, form submission, and action in the UI that
        // changes state goes through here.
        //
        // Flow:
        //   1. Show busy indicator
        //   2. Dispatch command through the pipeline
        //      (Logging → Authorization → Validation → Transaction → Handler)
        //   3. On success: return result
        //   4. On validation failure: show field errors as notification
        //   5. On known application error: show message as notification
        //   6. On unexpected error: show generic message, log full details
        //   7. Always: hide busy indicator
        // ══════════════════════════════════════════════════════════════
        protected async Task<TResult?> ExecuteCommandAsync<TCommand, TResult>(
            TCommand command,
            CancellationToken ct = default )
            where TCommand : ICommand<TResult>
        {
            try
            {
                BusyIndicator.Show();

                return await _commandDispatcher
                           .SendAsync<TCommand, TResult>(command, ct);
            }
            catch (ApplicationValidationException ex)
            {
                // Validation failures are expected user input errors.
                // Show each failure as a bullet point so the user knows
                // exactly what to correct.
                string errors = string.Join(
                    Environment.NewLine,
                    ex.Errors.Select(e => $"• {e.ErrorMessage}"));

                Notifications.ShowWarning(errors);
                return default;
            }
            catch (ApplicationExceptions ex)
            {
                // Known application-level errors — NotFound, Duplicate,
                // ConflictException, etc. These have meaningful messages
                // written for end users, not developers.
                Notifications.ShowError(ex.Message);
                return default;
            }
            catch (DomainException ex)
            {
                // Domain invariant violations — meaningful to the end user,
                // written by the aggregate itself (e.g. "Guest is already
                // unregistered", "Cannot cancel within the cancellation window").
                Notifications.ShowWarning(ex.Message);
                return default;
            }
            catch (Exception ex)
            {
                // Truly unexpected errors. Show a generic message to the
                // user — never expose stack traces in a desktop UI —
                // but log the full detail for diagnosis.
                Notifications.ShowError(
                    "An unexpected error occurred. Please try again or contact support.");

                Log.Error(
                    ex,
                    "Unhandled exception in {ViewModel}.ExecuteCommandAsync<{Command}>",
                    GetType()
                        .Name,
                    typeof(TCommand).Name);

                return default;
            }
            finally
            {
                // Always hide the busy indicator regardless of outcome.
                BusyIndicator.Hide();
            }
        }

        // Convenience overload for commands that return Unit (void commands).
        // Keeps calling code clean — no need to write <Unit> everywhere.
        protected async Task ExecuteCommandAsync<TCommand>(
            TCommand command,
            CancellationToken ct = default )
            where TCommand : ICommand
        {
            await ExecuteCommandAsync<TCommand, Unit>(command, ct);
        }
    }
}
