﻿using DevExpress.Mvvm.UI.Interactivity;
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
            AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;
        }

        void AssociatedObject_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            BranchViewModel selectedBranch = AssociatedObject.SelectedItem as BranchViewModel;
            if(selectedBranch == null)
                return;
            if(selectedBranch.Repository.Repositories.SelectedBranches.Contains(selectedBranch))
                return;
            selectedBranch.Repository.Repositories.SelectedBranches.Add(selectedBranch);
        }

        protected override void OnDetaching() {
            AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
            base.OnDetaching();
        }
    }
}
