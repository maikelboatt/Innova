namespace Innova.Presentation.Core.Services.Abstractions
{
    public interface INotificationService
    {
        void Show( string message, NotificationType type = NotificationType.Info );

        void ShowSuccess( string message );

        void ShowError( string message );

        void ShowWarning( string message );
    }
}
