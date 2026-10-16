using System.Windows;
using Innova.Presentation.Core.Identity;
using MvvmCross.Platforms.Wpf.Views;

namespace Innova.Presentation.WPF.Views.Identity
{
    public partial class LoginView:MvxWpfView
    {
        public LoginView()
        {
            InitializeComponent();
            PasswordInput.PasswordChanged += PasswordInput_PasswordChanged;
        }

        // Deliberate, narrow MVVM exception: PasswordBox.Password is not
        // a DependencyProperty (by WPF design, for security), so no pure
        // binding can reach it. This handler does nothing but forward the
        // current value to the ViewModel — no logic, no decision-making —
        // so it stays a thin plumbing bridge, not a violation of MVVM's
        // actual intent (keeping behavior out of code-behind).
        private void PasswordInput_PasswordChanged( object sender, RoutedEventArgs e )
        {
            if (DataContext is LoginViewModel viewModel)
                viewModel.Password = PasswordInput.Password;
        }

        private void CloseButton_OnClick( object sender, RoutedEventArgs e )
        {
            Window
                .GetWindow(this)
                ?.Close();
        }
    }
}
