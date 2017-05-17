using DevExpress.Mvvm;
using DXVcs2Git.Core.Configuration;

namespace DXVcs2Git.UI2{
    public interface IMainViewModel {
        Config Config { get; }
    }

    public class MainViewModel : ViewModelBase, IMainViewModel {
        public Config Config { get; }

        public MainViewModel() {
            Config = ConfigSerializer.GetConfig();
        }
    }
}