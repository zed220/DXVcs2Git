using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace DXVcs2Git.UI2 {
    public interface IMergeRequestViewModel {
        BranchViewModel Branch { get; }
    }

    public class MergeRequestViewModel : ViewModelWorkerBase, IMergeRequestViewModel {
        public MergeRequest MergeRequest { get; private set; }
        public BranchViewModel Branch { get; private set; }
        public string SourceBranch { get; private set; }
        public string TargetBranch { get; private set; }
        public string Author { get; private set; }
        public string Assignee { get; private set; }
        public bool SupportsTesting {
            get { return GetProperty(() => SupportsTesting); }
            private set { SetProperty(() => SupportsTesting, value); }
        }
        public bool IsModified {
            get { return GetProperty(() => IsModified); }
            private set { SetProperty(() => IsModified, value); }
        }
        public string Title {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value, TitleChanged); }
        }
        public string Description {
            get { return GetProperty(() => Description); }
            set { SetProperty(() => Description, value, DescriptionChanged); }
        }
        public ObservableCollection<CommitViewModel> Commits {
            get { return GetProperty(() => Commits); }
            private set { SetProperty(() => Commits, value); }
        }
        public ObservableCollection<MergeRequestFileDataViewModel> Changes {
            get { return GetProperty(() => Changes); }
            private set { SetProperty(() => Changes, value); }
        }

        protected override void OnParameterChanged(object parameter) {
            UpdateMergeRequest();
        }

        public BranchViewModel MergeParameter { get { return Parameter as BranchViewModel; } }

        public MergeRequestViewModel() { }

        void UpdateMergeRequest() {
            Branch = MergeParameter;
            MergeRequest = Branch.MergeRequest;
            if(MergeRequest == null) {
                Title = null;
                SourceBranch = null;
                TargetBranch = null;
                Author = null;
                Assignee = null;
                SupportsTesting = false;
                return;
            }
            
            Title = MergeRequest.Title;
            Description = MergeRequest.Description;
            SourceBranch = MergeRequest.SourceBranch;
            TargetBranch = MergeRequest.TargetBranch;
            Author = MergeRequest.Author.Username;
            Assignee = MergeRequest.Assignee?.Username;
            SupportsTesting = Branch?.SupportsTesting ?? false;
            UpdateContent();
        }

        void UpdateContent() {
            Task.Run(() => {
                IsLoading = true;
                UpdateCommits();
                LoadChanges();
                IsLoading = false;
            });
        }

        void UpdateCommits() {
            if(!SupportsTesting) {
                //maybe remove commits
                return;
            }
            if(Branch.MergeRequest == null) {
                return;
            }
            Task.Run(() => LoadCommits());
        }

        async void LoadCommits() {
            Commits = new ObservableCollection<CommitViewModel>();
            //Commits = Enumerable.Empty<CommitViewModel>();
            (await Branch.GetCommits()).ToList().ForEach(c => Commits.Add(new CommitViewModel(c)));
            await Task.Run(() => {
                List<Task> loadCommitsTaskList = new List<Task>();
                foreach (var commit in Commits) {
                    loadCommitsTaskList.Add(Task.Run(() => commit.Update(Branch)));
                }
                //commits.ForEach(c => Commits.Add(CommitViewModel.Create(c, Branch)));
                Task.WaitAll(loadCommitsTaskList.ToArray());
            });
        }
        async void LoadChanges() {
            Changes = new ObservableCollection<MergeRequestFileDataViewModel>();
            (await Branch.GetMergeRequestChanges()).ToList().ForEach(c => Changes.Add(new MergeRequestFileDataViewModel(c)));
        }

        public void Apply() {
            if(!IsModified)
                return;

        }

        void DescriptionChanged() {
            if(Description == MergeRequest.Description)
                return;
            IsModified = true;
        }
        void TitleChanged() {
            if(Title == MergeRequest.Title)
                return;
            IsModified = true;
        }
    }
}
