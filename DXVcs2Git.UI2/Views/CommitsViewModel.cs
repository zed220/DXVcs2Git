using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public interface ICommitsViewModel : ISupportParameter, IWorker {

    }

    public class CommitsViewModel : ViewModelWorkerBase, ICommitsViewModel {
        public ObservableCollection<CommitViewModel> Commits {
            get { return GetProperty(() => Commits); }
            private set { SetProperty(() => Commits, value); }
        }

        public BranchViewModel Branch { get { return Parameter as BranchViewModel; } }

        protected override void OnParameterChanged(object parameter) {
            if(IsLoading)
                return;
            Task.Run(() => {
                IsLoading = true;
                UpdateCommits();
                IsLoading = false;
            });
        }

        async void UpdateCommits() {
            if(Branch == null) {
                Commits = new ObservableCollection<CommitViewModel>();
                return;
            }
            var commits = new ObservableCollection<CommitViewModel>();
            (await Branch.GetCommits()).ToList().ForEach(c => commits.Add(new CommitViewModel(c)));
            await Task.Run(() => {
                List<Task> loadCommitsTaskList = new List<Task>();
                foreach(var commit in commits) {
                    loadCommitsTaskList.Add(Task.Run(() => commit.Update(Branch)));
                }
                Task.WaitAll(loadCommitsTaskList.ToArray());
            });
            Commits = commits;
        }
    }
}
