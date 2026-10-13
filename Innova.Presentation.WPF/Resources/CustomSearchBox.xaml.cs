using System.Windows;
using MahApps.Metro.IconPacks;
using MvvmCross.Platforms.Wpf.Views;

namespace Innova.Presentation.WPF.Resources
{
    public partial class CustomSearchBox:MvxWpfView
    {
        // 🏷️ Placeholder text shown when empty
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(
                nameof(Placeholder),
                typeof(string),
                typeof(CustomSearchBox),
                new PropertyMetadata("Search..."));

        // 💬 The main data text (user input)
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(
                nameof(Data),
                typeof(string),
                typeof(CustomSearchBox),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // 🔍 Optional: The icon used (default is Magnify)
        public static readonly DependencyProperty IconKindProperty =
            DependencyProperty.Register(
                nameof(IconKind),
                typeof(PackIconMaterialKind),
                typeof(CustomSearchBox),
                new PropertyMetadata(PackIconMaterialKind.Magnify));

        public CustomSearchBox()
        {
            InitializeComponent();
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Data
        {
            get => (string)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public PackIconMaterialKind IconKind
        {
            get => (PackIconMaterialKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }
    }
}
