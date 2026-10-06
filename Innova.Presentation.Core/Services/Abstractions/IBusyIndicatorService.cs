namespace Innova.Presentation.Core.Services.Abstractions
{
    public interface IBusyIndicatorService
    {
        bool IsBusy { get; }

        void Show( string message = "Loading..." );

        void Hide();

        event EventHandler<bool> BusyStateChanged;
    }
}
