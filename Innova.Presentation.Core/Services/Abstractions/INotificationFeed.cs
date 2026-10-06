using System.Collections.ObjectModel;

namespace Innova.Presentation.Core.Services.Abstractions
{
    /// <summary>
    ///     Read-side companion to INotificationService. Deliberately
    ///     separate — INotificationService is what every ViewModel in
    ///     the app depends on to FIRE notifications (write-only by
    ///     design, so a form ViewModel can never accidentally read or
    ///     clear another screen's toasts). INotificationFeed is what
    ///     the ONE toast-host View (ShellView) depends on to actually
    ///     RENDER them. Same underlying NotificationService instance
    ///     implements both; registered against both interfaces as the
    ///     same singleton.
    /// </summary>
    public interface INotificationFeed
    {
        ObservableCollection<NotificationItem> Notifications { get; }

        void Dismiss( Guid id );
    }
}
