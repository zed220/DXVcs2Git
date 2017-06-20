using DevExpress.Mvvm;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DXVcs2Git.UI2 {
    public abstract class ViewModelWorkerBase : ViewModelBase, IWorker {
        protected readonly IMainViewModel MainViewModel;

        public ViewModelWorkerBase(IMainViewModel mainViewModel) {
            MainViewModel = mainViewModel;
        }

        public virtual bool IsLoading {
            get { return GetProperty(() => IsLoading); }
            private set { SetProperty(() => IsLoading, value, OnLoadingChanged); }
        }

        protected virtual void OnLoadingChanged() {
            lock(this) {
                if(IsLoading)
                    MainViewModel.WorkStarted(this);
                else
                    MainViewModel.WorkFinished(this);
            }
        }

        protected bool StartLoading() {
            if(IsLoading)
                return false;
            lock(this) {
                if(IsLoading)
                    return false;
                IsLoading = true;
            }
            return true;
        }
        protected void EndLoading() {
            IsLoading = false;
        }
    }
}
