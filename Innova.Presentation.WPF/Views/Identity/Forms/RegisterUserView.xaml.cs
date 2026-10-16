using System.Windows;
using Innova.Presentation.Core.Identity.Forms;
using MvvmCross.Platforms.Wpf.Views;

namespace Innova.Presentation.WPF.Views.Identity.Forms
{
    public partial class RegisterUserView:MvxWpfView
    {
        public RegisterUserView()
        {
            InitializeComponent();
            PasswordInput.PasswordChanged += PasswordInput_PasswordChanged;
        }

        private void PasswordInput_PasswordChanged( object sender, RoutedEventArgs e )
        {
            if (DataContext is RegisterUserViewModel viewModel)
                viewModel.Password = PasswordInput.Password;
        }
    }
}
