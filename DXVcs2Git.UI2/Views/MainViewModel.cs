using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.UI;
using DXVcs2Git.Core.Configuration;
using Microsoft.Practices.ServiceLocation;
using DevExpress.Xpf.Core;

namespace DXVcs2Git.UI2{
    public interface IMainViewModel {
        Config Config { get; }
    }

    public class MainViewModel : ViewModelBase, IMainViewModel {
        public Config Config { get; private set; }

        public ScrollBarMode ScrollBarMode {
            get { return GetProperty(() => ScrollBarMode); }
            set { SetProperty(() => ScrollBarMode, value); }
        }

        public MainViewModel() {
            UpdateConfig();
        }

        public void ShowSettings() {
            ModuleManager.DefaultWindowManager.Show(Regions.Settings, Modules.SettingsView);
            ModuleManager.DefaultWindowManager.Clear(Regions.Settings);
            UpdateConfig();
        }

        void UpdateConfig() {
            var config = ConfigSerializer.GetConfig();
            if(Config != null && ConfigSerializer.IsConfigEquals(config, Config))
                return;
            Config = config;
            UpdateAppearance();
            UpdateContent();
        }

        void UpdateContent() {
            ModuleManager.DefaultManager.Clear(Regions.Content);
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.Content, Modules.RepositoriesViewContent);
        }

        void UpdateAppearance() {
            ScrollBarMode = (ScrollBarMode)Config.ScrollBarMode;
            ApplicationThemeHelper.ApplicationThemeName = Config?.DefaultTheme ?? Theme.DefaultThemeName;
        }
    }
}