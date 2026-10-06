namespace Innova.Presentation.Core.Services.Abstractions
{
    public interface IMessageBoxService
    {
        Task<bool> ShowConfirmAsync( string title, string message );

        Task ShowErrorAsync( string title, string message );

        Task ShowInfoAsync( string title, string message );

        Task ShowWarningAsync( string title, string message );
    }
}
