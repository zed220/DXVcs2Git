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
            //ModuleManager.DefaultManager.Clear(Regions.Navigation);
            //ModuleManager.DefaultManager.Clear(Regions.Ribbon);
            ModuleManager.DefaultManager.Clear(Regions.Content);
            //ModuleManager.DefaultManager.Clear(Regions.Colorizer);
            //ModuleManager.DefaultManager.Clear(Regions.Preview);
            //ModuleManager.DefaultManager.Clear(Regions.OutputView);
            //ModuleManager.DefaultManager.Clear(Regions.MergeRequest);
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

    //public class MergeRequestMifRegistrator : IParameterizedMifRegistrator<MergeRequest> {
    //    //readonly IModuleManager Manager;

    //    public MergeRequestMifRegistrator(MergeRequest parameter) {
    //        Parameter = parameter;
    //        //Manager = new ModuleManager(ViewModelLocator.Default, ViewLocator.Default, null, true, false);
    //        //Manager.Register(Regions.MergeRequest, new Module(Modules.MergeRequestView, ServiceLocator.Current.GetInstance<IMergeRequestViewModel>, typeof(MergeRequestView)));
            
    //    }

    //    public MergeRequest Parameter { get; set; }

    //    public void Dispose() {
    //        ModuleManager.DefaultManager.Unregister(Regions.MergeRequest + Parameter.ProjectId + Parameter.Id, Modules.MergeRequestView);
    //    }

    //    public bool LoadState(string logicalstate, string visualState) {
    //        return false;
    //    }

    //    public void RegisterUI() {
    //        ModuleManager.DefaultManager.Register(Regions.MergeRequest + Parameter.ProjectId + Parameter.Id, new Module(Modules.MergeRequestView, ServiceLocator.Current.GetInstance<IMergeRequestViewModel>, typeof(MergeRequestView)));
    //        ModuleManager.DefaultManager.InjectOrNavigate(Regions.MergeRequest + Parameter.ProjectId + Parameter.Id, Modules.MergeRequestView, Parameter);
    //    }

    //    public void Reset() {
            
    //    }

    //    //public List<string> Regions
    //}
}
