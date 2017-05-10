using DevExpress.Mvvm.ModuleInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public class RepositoriesViewMifRegistrator : IMifRegistrator {
        public void Dispose() {
            ModuleManager.DefaultManager.Clear(Regions.Navigation);
            ModuleManager.DefaultManager.Clear(Regions.Ribbon);
            ModuleManager.DefaultManager.Clear(Regions.Content);
            //ModuleManager.DefaultManager.Clear(Regions.Colorizer);
            //ModuleManager.DefaultManager.Clear(Regions.Preview);
            //ModuleManager.DefaultManager.Clear(Regions.OutputView);
        }

        public void RegisterUI() {
            //ModuleManager.DefaultManager.InjectOrNavigate(Regions.Settings, Regions.Settings);
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.MainView, Modules.MainView);

            //ModuleManager.DefaultManager.InjectOrNavigate(Regions.Ribbon, Modules.RepositoriesViewRibbon);
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.Content, Modules.RepositoriesViewContent);
        }

        public bool LoadState(string logicalstate, string visualState) {
            return false;
        }

        public void Reset() {
        }
    }
}
