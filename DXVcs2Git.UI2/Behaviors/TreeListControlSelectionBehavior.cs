using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Accordion;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid.TreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Data.TreeList;

namespace DXVcs2Git.UI2 {
    public class TreeListControlSelectionBehavior : Behavior<TreeListControl> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += AssociatedObject_SelectedItemChanged;
        }

        async void AssociatedObject_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e) {
            RepositoryViewModel selectedRepository = e.NewItem as RepositoryViewModel;
            if(selectedRepository != null) {
                e.Handled = true;
                AssociatedObject.View.GetNodeByContent(selectedRepository).IsExpanded = true;
                return;
            }
            BranchViewModel selectedBranch = e.NewItem as BranchViewModel;
            if(selectedBranch == null)
                return;
            e.Handled = true;
            selectedBranch.RefreshMergeRequestAsync();
        }

        protected override void OnDetaching() {
            AssociatedObject.SelectedItemChanged -= AssociatedObject_SelectedItemChanged;
            base.OnDetaching();
        }
    }
}
