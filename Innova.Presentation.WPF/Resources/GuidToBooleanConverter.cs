using System.Globalization;
using System.Windows.Data;

namespace Innova.Presentation.WPF.Resources
{
    public sealed class GuidToBooleanConverter:IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture ) => value is Guid id && id != Guid.Empty;

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture ) => throw new NotSupportedException(
                                         $"{nameof(GuidToBooleanConverter)} supports one-way conversion only.");
    }
}
