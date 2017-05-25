using DevExpress.Mvvm;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public abstract class ViewModelWorkerBase : ViewModelBase, IWorker {
        public bool IsLoading {
            get { return GetProperty(() => IsLoading); }
            protected set { SetProperty(() => IsLoading, value, LoadingChanged); }
        }

        void LoadingChanged() {
            IMainViewModel vm = ServiceLocator.Current.GetInstance<IMainViewModel>();
            if(IsLoading)
                vm.WorkStarted(this);
            else
                vm.WorkFinished(this);
        }
    }
}
