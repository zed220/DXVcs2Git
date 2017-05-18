using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace DXVcs2Git.UI2 {
    public class RepositoryLoadingConverter : ConverterBase, IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if(values.Length != 2)
                return Visibility.Collapsed;
            bool? result = values[0].GetType().GetProperty("IsLoading")?.GetValue(values[0]) as bool?;
            return result.HasValue && result.Value ? Visibility.Visible : Visibility.Collapsed;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public abstract class ConverterBase : MarkupExtension {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return Activator.CreateInstance(GetType());
        }
    }
}
