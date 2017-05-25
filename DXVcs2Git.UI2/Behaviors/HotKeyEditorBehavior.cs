﻿using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors;
using DXVcs2Git.Core.Configuration;
using DXVcs2Git.UI2.NativeMethods;
using System.Windows.Input;

namespace DXVcs2Git.UI2 {
    public class HotKeyEditorBehavior : Behavior<TextEdit> {        
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.EditValue = ConfigSerializer.GetConfig().KeyGesture;
            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }
        protected override void OnDetaching() {
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            base.OnDetaching();
        }
      
        void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (e.Key == Key.Back)
                AssociatedObject.EditValue = null;
            else if (e.Key >= Key.A && e.Key < Key.Z)
                AssociatedObject.EditValue = HotKeyHelper.GetString(e.Key, e.KeyboardDevice.Modifiers);
            e.Handled = true;
        }        
    }
}
