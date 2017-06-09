using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DXVcs2Git.UI2 {
    public class ContainsMergeRequestConverter : ConverterBase, IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if(values.Length != 2)
                return Visibility.Collapsed;
            BranchViewModel branch = values[0] as BranchViewModel;
            if(branch == null)
                return Visibility.Collapsed;
            return values[1] == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
