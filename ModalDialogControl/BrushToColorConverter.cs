// ModalDialogControl/BrushToColorConverter.cs

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ModalDialogControl
{
    public sealed class BrushToColorConverter:IValueConverter
    {
        public object Convert( object value,
                               Type targetType,
                               object parameter,
                               CultureInfo culture ) => value is SolidColorBrush brush
                                                            ? brush.Color
                                                            : Colors.Black;

        public object ConvertBack( object value,
                                   Type targetType,
                                   object parameter,
                                   CultureInfo culture ) => throw new NotSupportedException();
    }
}
