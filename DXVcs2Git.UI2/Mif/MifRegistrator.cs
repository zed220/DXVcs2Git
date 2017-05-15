using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using Microsoft.Practices.ServiceLocation;
using System.Windows;

namespace DXVcs2Git.UI2 {
    public static class MifRegistrator {
        static MifRegistrator() {
            ViewModelLocator.Default = new VMLocator();
            ModuleManager.DefaultManager.Register(Regions.MainView, new Module(Modules.MainView, ServiceLocator.Current.GetInstance<IMainViewModel>, typeof(MainView)));
            ModuleManager.DefaultManager.Register(Regions.Content, new Module(Modules.RepositoriesViewContent, ServiceLocator.Current.GetInstance<IRepositoriesViewModel>, typeof(RepositoriesView)));
            //ModuleManager.DefaultManager.Register(Regions.MergeRequest, new Module(Modules.MergeRequestView, ServiceLocator.Current.GetInstance<IMergeRequestViewModel>, typeof(MergeRequestView)));
            //ServiceLocator.
        }

        public static void Register() {
        }
    }

    public class VMLocator : ViewModelLocator, IViewModelLocator {
        public VMLocator() : base(Application.Current) { }
        object IViewModelLocator.ResolveViewModel(string name) {
            return ServiceLocator.Current.GetInstance(this.ResolveViewModelType(name));
        }
    }


}