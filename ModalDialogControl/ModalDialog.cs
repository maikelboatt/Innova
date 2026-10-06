// ModalDialogControl/ModalDialog.cs

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModalDialogControl
{
    public class ModalDialog:ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(ModalDialog),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TheBackgroundProperty = DependencyProperty.Register(
            nameof(TheBackground),
            typeof(SolidColorBrush),
            typeof(ModalDialog),
            new PropertyMetadata(default(SolidColorBrush)));

        public static readonly DependencyProperty TheShadowProperty = DependencyProperty.Register(
            nameof(TheShadow),
            typeof(SolidColorBrush),
            typeof(ModalDialog),
            new PropertyMetadata(default(SolidColorBrush)));

        // New — Border doesn't have this by default; exposed so each
        // consuming app can match its own window chrome radius.
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(ModalDialog),
            new PropertyMetadata(new CornerRadius(5)));

        static ModalDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalDialog), new FrameworkPropertyMetadata(typeof(ModalDialog)));

            // Preserves MediCore's existing look (600×400 min, 1000×800 max)
            // as the DEFAULT, while making these genuinely overridable
            // per-instance now that the template actually listens to them
            // (see Bug B fix in the style below).
            MinWidthProperty.OverrideMetadata(typeof(ModalDialog), new FrameworkPropertyMetadata(600d));
            MinHeightProperty.OverrideMetadata(typeof(ModalDialog), new FrameworkPropertyMetadata(400d));
            MaxWidthProperty.OverrideMetadata(typeof(ModalDialog), new FrameworkPropertyMetadata(1000d));
            MaxHeightProperty.OverrideMetadata(typeof(ModalDialog), new FrameworkPropertyMetadata(800d));
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public SolidColorBrush TheBackground
        {
            get => (SolidColorBrush)GetValue(TheBackgroundProperty);
            set => SetValue(TheBackgroundProperty, value);
        }

        public SolidColorBrush TheShadow
        {
            get => (SolidColorBrush)GetValue(TheShadowProperty);
            set => SetValue(TheShadowProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
    }
}
