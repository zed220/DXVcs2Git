using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.UI;
using DevExpress.Xpf.Layout.Core;
using Microsoft.Practices.ServiceLocation;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public class RepositoriesViewMifRegistrator : IMifRegistrator {
        public void Dispose() {
            ModuleManager.DefaultManager.Clear(Regions.MainView);
        }

        public void RegisterUI() {
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.MainView, Modules.MainView);
        }

        public bool LoadState(string logicalstate, string visualState) {
            return false;
        }

        public void Reset() {
        }
    }
}
