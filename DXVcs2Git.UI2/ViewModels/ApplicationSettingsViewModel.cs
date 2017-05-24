using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Xpf.Core;
using DXVcs2Git.Core.Configuration;
using DXVcs2Git.Core.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public interface IApplicationSettingsViewModel { }

    public class ApplicationSettingsViewModel : ViewModelBase, IApplicationSettingsViewModel {
        readonly Config Config;
        //readonly RepoConfigsReader ConfigsReader;

        #region Properties
        public string DefaultTheme {
            get { return GetProperty(() => DefaultTheme); }
            set { SetProperty(() => DefaultTheme, value, () => Config.DefaultTheme = value); }
        }
        public ScrollBarMode ScrollBarMode {
            get { return GetProperty(() => ScrollBarMode); }
            set { SetProperty(() => ScrollBarMode, value, () => Config.ScrollBarMode = (int)value); }
        }
        //public bool StartWithWindows {
        //    get { return GetProperty(() => StartWithWindows); }
        //    set { SetProperty(() => StartWithWindows, value); }
        //}
        public string KeyGesture {
            get { return GetProperty(() => KeyGesture); }
            set { SetProperty(() => KeyGesture, value, () => Config.KeyGesture = value); }
        }
        public bool SupportsTesting {
            get { return GetProperty(() => SupportsTesting); }
            set { SetProperty(() => SupportsTesting, value, () => Config.SupportsTesting = value); }
        }
        public bool TestByDefault {
            get { return GetProperty(() => TestByDefault); }
            set { SetProperty(() => TestByDefault, value, () => Config.TestByDefault = value); }
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
            set { SetProperty(() => AlwaysSure4, value, () => Config.AlwaysSure = value); }
        }
        #endregion

        public ApplicationSettingsViewModel(ISettingsViewModel settingsViewModel) {
            Config = settingsViewModel.Config;
            InitializeDefaults();
            //ModuleManager.DefaultManager.GetEvents(this).NavigatedAway += ApplicationSettingsViewModel_NavigatedAway;
        }

        //private void ApplicationSettingsViewModel_NavigatedAway(object sender, NavigationEventArgs e) {
        //    Config.KeyGesture = KeyGesture;
        //    Config.SupportsTesting = SupportsTesting;
        //    Config.DefaultTheme = DefaultTheme;
        //    Config.ScrollBarMode = (int)ScrollBarMode;
        //    Config.AlwaysSure = AlwaysSure4;
        //    Config.TestByDefault = TestByDefault;
        //}

        void InitializeDefaults() {
            KeyGesture = Config.KeyGesture;
            SupportsTesting = Config.SupportsTesting;
            DefaultTheme = Config.DefaultTheme;
            ScrollBarMode = (ScrollBarMode)Config.ScrollBarMode;
            AlwaysSure4 = AlwaysSure3 = AlwaysSure2 = AlwaysSure1 = Config.AlwaysSure;
            TestByDefault = Config.TestByDefault;
        }

        public override string ToString() {
            return "Application";
        }
    }
}
