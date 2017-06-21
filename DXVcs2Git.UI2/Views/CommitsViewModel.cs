using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DXVcs2Git.UI2 {
    public interface ICommitsViewModel : ISupportParameter, IWorker {

    }

    public class CommitsViewModel : ViewModelWorkerBase, ICommitsViewModel {
        public CommitsViewModel(IMainViewModel mainViewModel) : base(mainViewModel) { }

        public ObservableCollection<CommitViewModel> Commits {
            get { return GetProperty(() => Commits); }
            private set { SetProperty(() => Commits, value); }
        }

        public BranchViewModel Branch { get { return Parameter as BranchViewModel; } }

        protected override void OnParameterChanged(object parameter) {
            if(!StartLoading())
                return;
            Task.Run(() => UpdateCommits());
        }

        void UpdateCommits() {
            Commits = new ObservableCollection<CommitViewModel>();
            if(Branch == null) {
                return;
            }
            var commits = new List<CommitViewModel>();
            Branch.GetCommits().ToList().ForEach(c => commits.Add(new CommitViewModel(c)));
            App.Current.Dispatcher.BeginInvoke(new Action(() => { Commits = new ObservableCollection<CommitViewModel>(commits); }));
            List<Task> loadCommitsTaskList = new List<Task>();
            foreach(var commit in commits)
                loadCommitsTaskList.Add(Task.Run(() => commit.Update(Branch)));
            Task.WaitAll(loadCommitsTaskList.ToArray());
            EndLoading();
        }
    }
}
