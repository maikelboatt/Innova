using System.Windows;
using System.Windows.Controls;

namespace Innova.Presentation.WPF.Resources
{
    public partial class CustomComboBox:UserControl
    {
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            nameof(Data),
            typeof(object),
            typeof(CustomComboBox),
            new PropertyMetadata(default(object)));

        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register(
            nameof(Placeholder),
            typeof(string),
            typeof(CustomComboBox),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ChosenValueProperty = DependencyProperty.Register(
            nameof(ChosenValue),
            typeof(object),
            typeof(CustomComboBox),
            new PropertyMetadata(default(object)));

        public CustomComboBox()
        {
            InitializeComponent();
        }

        public object ChosenValue
        {
            get => GetValue(ChosenValueProperty);
            set => SetValue(ChosenValueProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public object Data
        {
            get => (object)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}
