using System.Collections.ObjectModel;
using Innova.Presentation.Core.Services.Abstractions;
using MvvmCross.Base;

namespace Innova.Presentation.Core.Services
{
    public sealed record NotificationItem( Guid Id, string Message, NotificationType Type );

    /// <summary>
    ///     Toast-style notifications: items are added to an observable
    ///     collection and auto-removed after a delay. ShellView (or a
    ///     dedicated ToastHost control within it) binds an ItemsControl
    ///     to Notifications to render them — Notifications isn't part of
    ///     the interface since only the View needs it, not other
    ///     ViewModels.
    /// </summary>
    public sealed class NotificationService:INotificationService, INotificationFeed
    {
        private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(4);
        private static readonly TimeSpan ErrorDuration = TimeSpan.FromSeconds(7);

        private readonly IMvxMainThreadAsyncDispatcher _dispatcher;

        public NotificationService( IMvxMainThreadAsyncDispatcher dispatcher ) => _dispatcher = dispatcher;

        public ObservableCollection<NotificationItem> Notifications { get; } = [];

        // ══════════════════════════════════════════════════════════════
        // Dismiss — manual close (toast's own X button). Fire-and-forget
        // dispatch to the UI thread for the same reason Show() does:
        // ObservableCollection mutations must happen there, and callers
        // (a Button.Command in the toast template) don't await this.
        // ══════════════════════════════════════════════════════════════
        public void Dismiss( Guid id )
        {
            _ = _dispatcher.ExecuteOnMainThreadAsync(() =>
            {
                NotificationItem? item = Notifications.FirstOrDefault(n => n.Id == id);
                if (item is not null)
                    Notifications.Remove(item);

                return Task.CompletedTask;
            });
        }

        public void Show( string message, NotificationType type = NotificationType.Info )
        {
            NotificationItem item = new(Guid.NewGuid(), message, type);

            _ = _dispatcher.ExecuteOnMainThreadAsync(async () =>
            {
                Notifications.Add(item);

                TimeSpan duration = type == NotificationType.Error
                                        ? ErrorDuration
                                        : DefaultDuration;
                await Task.Delay(duration);

                // Guard: the user may have already dismissed this toast
                // manually (see Dismiss below) before the delay elapsed.
                if (Notifications.Contains(item))
                    Notifications.Remove(item);
            });
        }

        public void ShowSuccess( string message ) => Show(message, NotificationType.Success);

        public void ShowError( string message ) => Show(message, NotificationType.Error);

        public void ShowWarning( string message ) => Show(message, NotificationType.Warning);
    }
}
