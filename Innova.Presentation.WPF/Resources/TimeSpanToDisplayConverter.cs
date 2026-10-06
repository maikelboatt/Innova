using System.Globalization;
using System.Windows.Data;

namespace Innova.Presentation.WPF.Resources
{
    public sealed class TimeSpanToDisplayConverter:IValueConverter
    {
        public object Convert( object value,
                               Type targetType,
                               object parameter,
                               CultureInfo culture ) => value is TimeSpan ts
                                                            ? DateTime
                                                              .Today.Add(ts)
                                                              .ToString("h:mm tt", culture)
                                                            : string.Empty;

        public object ConvertBack( object value,
                                   Type targetType,
                                   object parameter,
                                   CultureInfo culture ) => throw new NotSupportedException();
    }
}
