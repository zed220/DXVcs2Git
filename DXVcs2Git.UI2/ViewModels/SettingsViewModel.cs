using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Xpf.Core;
using DXVcs2Git.Core.Configuration;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public interface ISettingsViewModel { }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel {
        readonly Config Config;

        #region Properties
        public string DefaultTheme {
            get { return GetProperty(() => DefaultTheme); }
            set { SetProperty(() => DefaultTheme, value); }
        }
        public ScrollBarMode ScrollBarMode {
            get { return GetProperty(() => ScrollBarMode); }
            set { SetProperty(() => ScrollBarMode, value); }
        }
        public bool SupportsTesting {
            get { return GetProperty(() => SupportsTesting); }
            set { SetProperty(() => SupportsTesting, value); }
        }
        public bool TestByDefault {
            get { return GetProperty(() => TestByDefault); }
            set { SetProperty(() => TestByDefault, value); }
        }
        public bool StartWithWindows {
            get { return GetProperty(() => StartWithWindows); }
            set { SetProperty(() => StartWithWindows, value); }
        }
        public string KeyGesture {
            get { return GetProperty(() => KeyGesture); }
            set { string oldValue = KeyGesture; SetProperty(() => KeyGesture, value); }
        }
        public bool AlwaysSure1 {
            get { return GetProperty(() => AlwaysSure1); }
            set { SetProperty(() => AlwaysSure1, value, () => { AlwaysSure2 &= AlwaysSure1; AlwaysSure3 &= AlwaysSure2; AlwaysSure4 &= AlwaysSure3; }); }
        }
        public bool AlwaysSure2 {
            get { return GetProperty(() => AlwaysSure2); }
            set { SetProperty(() => AlwaysSure2, value, () => { AlwaysSure3 &= AlwaysSure2; AlwaysSure4 &= AlwaysSure3; }); }
        }
        public bool AlwaysSure3 {
            get { return GetProperty(() => AlwaysSure3); }
            set { SetProperty(() => AlwaysSure3, value, () => { AlwaysSure4 &= AlwaysSure3; }); }
        }
        public bool AlwaysSure4 {
            get { return GetProperty(() => AlwaysSure4); }
            set { SetProperty(() => AlwaysSure4, value); }
        }
        public bool CommonXaml {
            get { return GetProperty(() => CommonXaml); }
            set { SetProperty(() => CommonXaml, value); }
        }
        public bool DiagramXaml {
            get { return GetProperty(() => DiagramXaml); }
            set { SetProperty(() => DiagramXaml, value); }
        }
        public bool XPFGITXaml {
            get { return GetProperty(() => XPFGITXaml); }
            set { SetProperty(() => XPFGITXaml, value); }
        }
        #endregion

        public SettingsViewModel(IMainViewModel mainViewModel) {
            Config = mainViewModel.Config.Clone();
            ModuleManager.DefaultManager.GetEvents(this).ViewModelRemoving += SettingsViewModel_ViewModelRemoving;
        }

        void SettingsViewModel_ViewModelRemoving(object sender, ViewModelRemovingEventArgs e) {
            MessageResult? result = GetService<IMessageBoxService>()?.ShowMessage("Save changes?", "Save changes?", MessageButton.YesNoCancel);
            if(!result.HasValue || result.Value == MessageResult.Cancel || result.Value == MessageResult.None) {
                e.Cancel = true;
                return;
            }
            if(result.Value == MessageResult.Yes) {
                //save
            }
        }
    }
}
