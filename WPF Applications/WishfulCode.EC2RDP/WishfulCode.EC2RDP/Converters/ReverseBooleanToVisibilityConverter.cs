using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;


namespace WishfulCode.EC2RDP.Converters
{
    public class ReverseBooleanToVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var boolValue = value as bool?;
            if (boolValue != null && boolValue.HasValue)
            {
                return (boolValue.Value) ? Visibility.Collapsed : Visibility.Visible;
            }
            throw new ArgumentException("Not a valid boolean type", "value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
       {
           return ((Visibility)value == Visibility.Visible) ? true : false;
        }
    }
}
