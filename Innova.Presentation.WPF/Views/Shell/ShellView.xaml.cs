using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MvvmCross.Platforms.Wpf.Views;

namespace Innova.Presentation.WPF.Views.Shell
{
    public partial class ShellView:MvxWpfView
    {
        private Window? _parentWindow;

        public ShellView()
        {
            InitializeComponent();
        }

        private void ShellView_OnLoaded( object sender, RoutedEventArgs e )
        {
            _parentWindow = Window.GetWindow(this);

            if (_parentWindow is not null)
            {
                _parentWindow.StateChanged += ParentWindow_OnStateChanged;
                UpdateRestoreIcon();
            }
        }

        // Keeps the Maximize/Restore icon correct regardless of HOW the
        // window state changed — button click, double-click title bar,
        // Aero snap, Win+Up/Down — not just the RestoreButton click path.
        private void ParentWindow_OnStateChanged( object? sender, EventArgs e )
        {
            UpdateRestoreIcon();
        }

        private void UpdateRestoreIcon()
        {
            if (_parentWindow is null) return;

            bool isMaximized = _parentWindow.WindowState == WindowState.Maximized;

            RestoreIcon.Data = (Geometry)FindResource(
                isMaximized
                    ? "IconWindowRestore"
                    : "IconWindowMaximize");

            RestoreButton.ToolTip = isMaximized
                                        ? "Restore"
                                        : "Maximise";
        }

        private void CloseButton_OnClick( object sender, RoutedEventArgs e ) => _parentWindow?.Close();

        private void MinimizeButton_OnClick( object sender, RoutedEventArgs e )
        {
            if (_parentWindow is not null)
                _parentWindow.WindowState = WindowState.Minimized;
        }

        private void RestoreButton_OnClick( object sender, RoutedEventArgs e )
        {
            if (_parentWindow is null) return;

            _parentWindow.WindowState = _parentWindow.WindowState == WindowState.Maximized
                                            ? WindowState.Normal
                                            : WindowState.Maximized;

            // UpdateRestoreIcon() also fires via StateChanged above, so this
            // explicit call is redundant but harmless — kept for clarity.
        }

        private void LogoArea_OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _parentWindow?.DragMove();
        }

        private void TopBar_OnMouseDown( object sender, MouseButtonEventArgs e )
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _parentWindow?.DragMove();
        }
    }
}
