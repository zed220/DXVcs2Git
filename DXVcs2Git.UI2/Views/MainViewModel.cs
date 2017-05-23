using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.UI;
using DXVcs2Git.Core.Configuration;
using Microsoft.Practices.ServiceLocation;

namespace DXVcs2Git.UI2{
    public interface IMainViewModel {
        Config Config { get; }
    }

    public class MainViewModel : ViewModelBase, IMainViewModel {
        public Config Config { get; }

        public MainViewModel() {
            Config = ConfigSerializer.GetConfig();
        }

        public void ShowSettings() {
            ModuleManager.DefaultWindowManager.Show(Regions.Settings, Modules.SettingsView);
            ModuleManager.DefaultWindowManager.Clear(Regions.Settings);
        }
    }
}