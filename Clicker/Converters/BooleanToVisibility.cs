using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Clicker
{
    public class BooleanToVisibility : BaseValueConverter<BooleanToVisibility>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                if ((bool)value)
                    return Visibility.Hidden;
                else
                    return Visibility.Visible;
            }
            else
            {
                if ((bool)value)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
