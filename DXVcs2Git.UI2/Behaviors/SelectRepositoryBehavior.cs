using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Accordion;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXVcs2Git.UI2 {
    public class SelectBranchBehavior : Behavior<TreeListControl> {
        protected override void OnAttached() {
            base.OnAttached();
            //AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
            AssociatedObject.SelectedItemChanged += AssociatedObject_SelectedItemChanged;
        }

        async void AssociatedObject_SelectedItemChanged(object sender, SelectedItemChangedEventArgs e) {
            RepositoryViewModel selectedRepository = e.NewItem as RepositoryViewModel;
            if(selectedRepository != null) {
                e.Handled = true;
                await selectedRepository.LoadBranchesAsync();
                AssociatedObject.View.GetNodeByContent(selectedRepository).IsExpanded = true;
                return;
            }
            BranchViewModel selectedBranch = e.NewItem as BranchViewModel;
            if(selectedBranch == null)
                return;
            e.Handled = true;
            selectedBranch.RefreshMergeRequestAsync();
            //throw new NotImplementedException();
        }

        //void AssociatedObject_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
        //    BranchViewModel selectedBranch = AssociatedObject.SelectedItem as BranchViewModel;
        //    if(selectedBranch == null)
        //        return;
        //    if(selectedBranch.Repository.Repositories.SelectedBranches.Contains(selectedBranch))
        //        return;
        //    selectedBranch.Repository.Repositories.SelectedBranches.Add(selectedBranch);
        //}

        protected override void OnDetaching() {
            AssociatedObject.SelectedItemChanged -= AssociatedObject_SelectedItemChanged;
            //AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
            base.OnDetaching();
        }
    }
}
