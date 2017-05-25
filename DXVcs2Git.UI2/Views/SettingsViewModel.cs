using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Xpf.Core;
using DXVcs2Git.Core.Configuration;
using DXVcs2Git.Core.Git;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public interface ISettingsViewModel {
        Config Config { get; }

        IEnumerable<UICommand> DialogCommands { get; }
    }

    public class SettingsViewModel : ViewModelBase, ISettingsViewModel {
        public Config Config { get; private set; }

        public IEnumerable<UICommand> DialogCommands { get; private set; }

        public SettingsViewModel() {
            ModuleManager.DefaultManager.GetEvents(this).ViewModelRemoving += SettingsViewModel_ViewModelRemoving;
            ModuleManager.DefaultManager.GetEvents(this).Navigated += SettingsViewModel_Navigated;
            ModuleManager.DefaultManager.GetEvents(this).NavigatedAway += SettingsViewModel_NavigatedAway;
            CreateCommands();
        }

        void SettingsViewModel_NavigatedAway(object sender, NavigationEventArgs e) {
            ModuleManager.DefaultManager.Clear(Regions.SettingsContent);
        }

        void SettingsViewModel_Navigated(object sender, NavigationEventArgs e) {
            Config = ConfigSerializer.GetConfig();
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.SettingsContent, Modules.ApplicationSettingsView);
            ModuleManager.DefaultManager.Inject(Regions.SettingsContent, Modules.RepositorySettingsView);
        }

        void CreateCommands() {
            List<UICommand> dialogCommands = new List<UICommand>();
            dialogCommands.Add(new UICommand() { IsDefault = true, Command = new DelegateCommand(Save), Caption = DXMessageBoxLocalizer.GetString(DXMessageBoxStringId.Ok) });
            dialogCommands.Add(new UICommand() { IsCancel = true, Caption = DXMessageBoxLocalizer.GetString(DXMessageBoxStringId.Cancel) });
            DialogCommands = dialogCommands;
        }


        bool IsChanged() {
            return !ConfigSerializer.IsConfigEquals(ConfigSerializer.GetConfig(), Config);
        }
        void Save() {
            ConfigSerializer.SaveConfig(Config);
        }

        void SettingsViewModel_ViewModelRemoving(object sender, ViewModelRemovingEventArgs e) {
            if(!IsChanged())
                return;
            MessageResult? result = GetService<IMessageBoxService>()?.ShowMessage("Save changes?", "Save changes?", MessageButton.YesNoCancel);
            if(!result.HasValue || result.Value == MessageResult.Cancel || result.Value == MessageResult.None) {
                e.Cancel = true;
                return;
            }
            if(result.Value == MessageResult.Yes) {
                Save();
            }
        }
    }
}
