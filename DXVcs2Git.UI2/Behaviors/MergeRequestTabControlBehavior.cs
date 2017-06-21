using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DXVcs2Git.UI2 {
    public class MergeRequestTabControlBehavior : Behavior<DXTabControl> {
        public static readonly DependencyProperty ShowChangesCommandProperty = DependencyProperty.Register("ShowChangesCommand", typeof(ICommand), typeof(MergeRequestTabControlBehavior), new PropertyMetadata(null));


        public ICommand ShowChangesCommand {
            get { return (ICommand)GetValue(ShowChangesCommandProperty); }
            set { SetValue(ShowChangesCommandProperty, value); }
        }

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, TabControlSelectionChangedEventArgs e) {
            if(e.NewSelectedIndex == 1)
                ShowChangesCommand?.Execute(null);
        }
    }
}
