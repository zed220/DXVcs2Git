using DevExpress.Mvvm;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public abstract class ViewModelWorkerBase : ViewModelBase, IWorker {
        protected readonly IMainViewModel MainViewModel;

        public ViewModelWorkerBase(IMainViewModel mainViewModel) {
            MainViewModel = mainViewModel;
        }

        public virtual bool IsLoading {
            get { return GetProperty(() => IsLoading); }
            protected set { SetProperty(() => IsLoading, value, OnLoadingChanged); }
        }

        protected virtual void OnLoadingChanged() {
            if(IsLoading)
                MainViewModel.WorkStarted(this);
            else
                MainViewModel.WorkFinished(this);
        }
    }
}
