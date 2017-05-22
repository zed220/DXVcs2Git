using DevExpress.Data.TreeList;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;

namespace DXVcs2Git.UI2 {
    public class TreeListViewExpandFocusedNodeBehavior : Behavior<TreeListView> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.NodeChanged += View_NodeChanged;
        }

        void View_NodeChanged(object sender, TreeListNodeChangedEventArgs e) {
            if(AssociatedObject.FocusedNode != e.Node || !(e.Node.Content is RepositoryViewModel))
                return;
            switch(e.ChangeType) {
                case NodeChangeType.Content:
                    e.Node.IsExpanded = true;
                    break;
            }
        }

        protected override void OnDetaching() {
            AssociatedObject.NodeChanged -= View_NodeChanged;
            base.OnDetaching();
        }
    }
}
