using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace DXVcs2Git.UI2 {
    public static class Bootstrapper {
        static IUnityContainer RootContainer { get; } = new UnityContainer();

        static Bootstrapper(){
            RootContainer.RegisterType<IMainViewModel, MainViewModel>(new ContainerControlledLifetimeManager());
            RootContainer.RegisterType<IRepositoriesViewModel, RepositoriesViewModel>(new ContainerControlledLifetimeManager());
            RootContainer.RegisterType<IMifRegistrator, RepositoriesViewMifRegistrator>(new ContainerControlledLifetimeManager());
            //RootContainer.RegisterType<IMergeRequestViewModel, MergeRequestViewModel>(new TransientLifetimeManager());
            RootContainer.RegisterType<IMergeRequestViewModel, MergeRequestViewModel>(new TransientLifetimeManager());
            ServiceLocator.SetLocatorProvider(() => new UnityServiceLocator(RootContainer));
            BuildMif();
        }

        public static void Run(){
            ServiceLocator.Current.GetInstance<IMifRegistrator>().RegisterUI();
        }

        static void BuildMif(){
            MifRegistrator.Register();
        }
    }
}