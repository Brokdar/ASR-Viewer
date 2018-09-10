using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Xml.Linq;

namespace BasicViews.Converter
{
    public class XAttributeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is XElement element))
                return Enumerable.Empty<XAttribute>();
            else if (!element.HasAttributes)
                return Enumerable.Empty<XAttribute>();

            return element.Attributes();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}