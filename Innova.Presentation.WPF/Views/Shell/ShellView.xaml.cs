using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MvvmCross.Platforms.Wpf.Views;

namespace Innova.Presentation.WPF.Views.Shell
{
    public partial class ShellView:MvxWpfView
    {
        private Window? _parentWindow;

        public ShellView()
        {
            InitializeComponent();
            // Subscribe to expander toggle events with mutual exclusion
            // Subscribe to expander toggle events with mutual exclusion
            RoomInventoryExpander.Checked += ( s, e ) =>
            {
                // Collapse Orders if it's expanded
                if (ReservationsExpander.IsChecked == true)
                {
                    ReservationsExpander.IsChecked = false;
                }
                ExpandSection(RoomInventorySubItems, RoomInventoryArrow);
            };
            RoomInventoryExpander.Unchecked += ( s, e ) => CollapseSection(RoomInventorySubItems, RoomInventoryArrow);

            ReservationsExpander.Checked += ( s, e ) =>
            {
                // Collapse Inventory if it's expanded
                if (RoomInventoryExpander.IsChecked == true)
                {
                    RoomInventoryExpander.IsChecked = false;
                }
                ExpandSection(ReservationSubItems, OrdersArrow);
            };
            ReservationsExpander.Unchecked += ( s, e ) => CollapseSection(ReservationSubItems, OrdersArrow);

            // Subscribe to toggle button state changes
            TgBtn.Checked += TgBtn_StateChanged;
            TgBtn.Unchecked += TgBtn_StateChanged;
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

#region Sub-Items Collapse and Expanse

        private void ExpandSection( StackPanel subItems, RotateTransform arrow )
        {
            // Show sub-items with animation
            subItems.Visibility = Visibility.Visible;

            // Animate arrow rotation
            DoubleAnimation rotateAnimation = new()
                                              {
                                                  From = 0,
                                                  To = 180,
                                                  Duration = TimeSpan.FromMilliseconds(200),
                                                  EasingFunction = new QuadraticEase
                                                                   {
                                                                       EasingMode = EasingMode.EaseOut
                                                                   }
                                              };
            arrow.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }

        private void CollapseSection( StackPanel subItems, RotateTransform arrow )
        {
            // Animate arrow rotation back
            DoubleAnimation rotateAnimation = new()
                                              {
                                                  From = 180,
                                                  To = 0,
                                                  Duration = TimeSpan.FromMilliseconds(200),
                                                  EasingFunction = new QuadraticEase
                                                                   {
                                                                       EasingMode = EasingMode.EaseOut
                                                                   }
                                              };
            arrow.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);

            // Hide sub-items after animation
            subItems.Visibility = Visibility.Collapsed;
        }

        private void TgBtn_StateChanged( object sender, RoutedEventArgs e )
        {
            // When collapsing, close any expanded sections
            if (TgBtn.IsChecked == false)
            {
                if (RoomInventoryExpander.IsChecked == true)
                {
                    RoomInventoryExpander.IsChecked = false;
                }
                if (ReservationsExpander.IsChecked == true)
                {
                    ReservationsExpander.IsChecked = false;
                }
            }
        }

#endregion

#region Title Bar Buttons

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

#endregion

#region Drag Events

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

#endregion

#region Nav-Items PopUp

        // Start: MenuLeft PopupButton //
        private void BtnDashboard_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnDashboard;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Dashboard";
        }

        private void BtnDashboard_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnGuest_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnGuest;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Guest";
        }

        private void BtnGuest_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnRoomInventory_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = RoomInventoryExpander;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Room Inventory";
        }

        private void BtnRoomInventory_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnRooms_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnRooms;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Rooms";
        }

        private void BtnRooms_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnRoomDefinitions_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnRoomDefinitions;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Room Definitions";
        }

        private void BtnRoomDefinitions_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnRoomAllotments_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnRoomAllotments;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Room Allotments";
        }

        private void BtnRoomAllotments_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnReservations_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = ReservationsExpander;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Reservations";
        }

        private void BtnReservations_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnSingleReservations_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnSingleReservations;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Single Reservations";
        }

        private void BtnSingleReservations_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnGroupBookings_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnGroupBookings;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Group Bookings";
        }

        private void BtnGroupBookings_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnStay_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnStay;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Stay";
        }

        private void BtnStay_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnHouseKeeping_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnHouseKeeping;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "HouseKeeping";
        }

        private void BtnHouseKeeping_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnBilling_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnBilling;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Billing";
        }

        private void BtnBilling_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void BtnSettings_MouseEnter( object sender, MouseEventArgs e )
        {
            if (TgBtn.IsChecked != false) return;
            Popup.PlacementTarget = BtnSettings;
            Popup.Placement = PlacementMode.Right;
            Popup.IsOpen = true;
            PopupHeader.PopupText.Text = "Settings";
        }

        private void BtnSettings_MouseLeave( object sender, MouseEventArgs e )
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

#endregion
    }
}
