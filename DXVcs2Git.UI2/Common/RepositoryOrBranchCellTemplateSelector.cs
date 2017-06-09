using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DXVcs2Git.UI2 {
    public class RepositoryOrBranchCellTemplateSelector : DataTemplateSelector {
        public DataTemplate RepositoryTemplate { get; set; }
        public DataTemplate BranchTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            EditGridCellData data = item as EditGridCellData;
            if(data != null) {
                object viewModel = data.RowData.Row;
                if(viewModel is RepositoryViewModel)
                    return RepositoryTemplate;
                if(viewModel is BranchViewModel)
                    return BranchTemplate;
            }
            return new DataTemplate();
        }
    }
}
