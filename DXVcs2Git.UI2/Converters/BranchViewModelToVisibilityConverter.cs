using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DXVcs2Git.UI2 {
    public class BranchViewModelToVisibilityConverter : ConverterBase, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is BranchViewModel;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class TypeConverter : ConverterBase, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null || parameter as Type == null)
                return null;
            return value.GetType() == (Type)parameter ? value : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
