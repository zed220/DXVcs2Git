using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace DXVcs2Git.UI2 {
    public abstract class ConverterBase : MarkupExtension {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return Activator.CreateInstance(GetType());
        }

    }

    public class MergeRequestRegionNameConverter : ConverterBase, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            BranchViewModel branch = (BranchViewModel)value;
            return Regions.MergeRequest + branch.Repository.Name + branch.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
