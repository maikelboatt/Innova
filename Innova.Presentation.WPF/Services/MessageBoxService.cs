// MediCore.Presentation.Wpf/Services/MessageBoxService.cs
//
// Lives in Presentation.Wpf, not Presentation.Core — it's a thin wrapper
// over System.Windows.MessageBox, a WPF-specific API. IMessageBoxService
// itself (the abstraction) can stay in Presentation.Core where it already
// is; only the concrete implementation needs to be WPF-specific, same
// reasoning as flagging IMessageBoxService's real home a few turns back.

using System.Windows;
using Innova.Presentation.Core.Services.Abstractions;

namespace Innova.Presentation.WPF.Services
{
    public sealed class MessageBoxService:IMessageBoxService
    {
        public Task<bool> ShowConfirmAsync( string title, string message )
        {
            Window? owner = System.Windows.Application.Current?.MainWindow;

            MessageBoxResult result = MessageBox.Show(
                owner,
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return Task.FromResult(result == MessageBoxResult.Yes);
        }

        public Task ShowErrorAsync( string title, string message )
        {
            Window? owner = System.Windows.Application.Current?.MainWindow;

            MessageBox.Show(
                owner,
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return Task.CompletedTask;
        }

        public Task ShowInfoAsync( string title, string message )
        {
            Window? owner = System.Windows.Application.Current?.MainWindow;

            MessageBox.Show(
                owner,
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return Task.CompletedTask;
        }

        public Task ShowWarningAsync( string title, string message )
        {
            Window? owner = System.Windows.Application.Current?.MainWindow;

            MessageBox.Show(
                owner,
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return Task.CompletedTask;
        }
    }
}
