using System.Windows;
using Innova.Presentation.Core.Identity.Forms;
using MvvmCross.Platforms.Wpf.Views;

namespace Innova.Presentation.WPF.Views.Identity.Forms
{
    public partial class ChangePasswordView:MvxWpfView
    {
        public ChangePasswordView()
        {
            InitializeComponent();
            CurrentPasswordInput.PasswordChanged += CurrentPasswordInputOnPasswordChanged;
            NewPasswordInput.PasswordChanged += NewPasswordInputOnPasswordChanged;
            ConfirmNewPasswordInput.PasswordChanged += ConfirmNewPasswordInputOnPasswordChanged;
        }

        private void CurrentPasswordInputOnPasswordChanged( object sender, RoutedEventArgs e )
        {
            if (DataContext is ChangePasswordViewModel viewModel)
                viewModel.CurrentPassword = CurrentPasswordInput.Password;
        }

        private void NewPasswordInputOnPasswordChanged( object sender, RoutedEventArgs e )
        {
            if (DataContext is ChangePasswordViewModel viewModel)
                viewModel.NewPassword = NewPasswordInput.Password;
        }

        private void ConfirmNewPasswordInputOnPasswordChanged( object sender, RoutedEventArgs e )
        {
            if (DataContext is ChangePasswordViewModel viewModel)
                viewModel.ConfirmNewPassword = ConfirmNewPasswordInput.Password;
        }
    }
}
