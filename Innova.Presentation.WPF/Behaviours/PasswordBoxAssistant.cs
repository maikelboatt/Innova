using System.Windows;
using System.Windows.Controls;

namespace Innova.Presentation.WPF.Behaviours
{
    /// <summary>
    ///     Bridges PasswordBox.Password (deliberately non-bindable by
    ///     WPF, for security) to a bindable attached property. Standard,
    ///     widely-used workaround pattern — not a security regression in
    ///     itself, since the plaintext still never touches the binding
    ///     engine's undo/tracing infrastructure, only this one-way sync.
    /// </summary>
    public static class PasswordBoxAssistant
    {
        public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.RegisterAttached(
            "BoundPassword",
            typeof(string),
            typeof(PasswordBoxAssistant),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnBoundPasswordChanged));

        public static string GetBoundPassword( DependencyObject d ) => (string)d.GetValue(BoundPasswordProperty);

        public static void SetBoundPassword( DependencyObject d, string value ) => d.SetCurrentValue(BoundPasswordProperty, value);

        private static void OnBoundPasswordChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
        {
            if (d is not PasswordBox passwordBox) return;

            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;

            if (passwordBox.Password != (string)e.NewValue)
                passwordBox.Password = (string)e.NewValue;

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private static void PasswordBox_PasswordChanged( object sender, RoutedEventArgs e )
        {
            if (sender is PasswordBox passwordBox)
                SetBoundPassword(passwordBox, passwordBox.Password);
        }
    }
}
