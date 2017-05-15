using DevExpress.Xpf.Core;
using System;
namespace DXVcs2Git.UI2 {
    public class BranchesDXTabControl : DXTabControl {
        public BranchesDXTabControl() {
        }

        protected override void OnAddItem(object newItem, int index) {
            base.OnAddItem(newItem, index);
            SelectedIndex = index;
        }
        protected override void OnRemoveItem(object oldItem, int index) {
            base.OnRemoveItem(oldItem, index);
            SelectedIndex = Math.Max(0, index - 1);
        }
    }
}
