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
using Microsoft.Practices.ServiceLocation;
using System.Windows;

namespace DXVcs2Git.UI2 {
    public interface IMergeRequestViewModel {
        BranchViewModel Branch { get; }
    }

    public class MergeRequestViewModel : ViewModelWorkerBase, IMergeRequestViewModel {
        public MergeRequest MergeRequest { get; private set; }
        public BranchViewModel Branch { get; private set; }
        public string SourceBranch { get; private set; }
        public string TargetBranch { get; private set; }

        public MergeRequestViewModel(IMainViewModel mainViewModel) : base(mainViewModel) { }

        public string Author {
            get { return GetProperty(() => Author); }
            private set { SetProperty(() => Author, value); }
        }
        public string Assignee {
            get { return GetProperty(() => Assignee); }
            private set { SetProperty(() => Assignee, value); }
        }
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
        public ICommitsViewModel CommitsViewModel {
            get { return GetProperty(() => CommitsViewModel); }
            private set { SetProperty(() => CommitsViewModel, value); }
        }
        public IChangesViewModel ChangesViewModel {
            get { return GetProperty(() => ChangesViewModel); }
            private set { SetProperty(() => ChangesViewModel, value); }
        }
        public bool PerformTesting {
            get { return GetProperty(() => PerformTesting); }
            set { SetProperty(() => PerformTesting, value, PerformTestingChanged); }
        }

        protected override void OnParameterChanged(object parameter) {
            UpdateMergeRequest();
        }

        public BranchViewModel MergeParameter { get { return Parameter as BranchViewModel; } }

        void UpdateMergeRequest() {
            if(IsLoading)
                return;
            if(ChangesViewModel != null && ChangesViewModel.IsLoading)
                return;
            if(CommitsViewModel != null && CommitsViewModel.IsLoading)
                return;
            IsLoading = true;
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
            IsLoading = false;
        }
        
        void UpdateContent() {
            Task.Run(() => {
                LoadCommits();
            });
            Task.Run(() => {
                LoadChanges();
            });
        }

        void LoadCommits() {
            if(!SupportsTesting)
                return;
            CommitsViewModel = ServiceLocator.Current.GetInstance<ICommitsViewModel>();
            CommitsViewModel.Parameter = Branch;
        }
        void LoadChanges() {
            ChangesViewModel = ServiceLocator.Current.GetInstance<IChangesViewModel>();
            ChangesViewModel.Parameter = Branch;
        }
        
        public void Apply() {
            if(!IsModified)
                return;
            if(GetService<IMessageBoxService>().Show("Are you sure?", "Update merge request", MessageBoxButton.OKCancel) != MessageBoxResult.OK)
                return;
            MergeRequest mergeRequest = Branch.UpdateMergeRequest(Title, Description, GetServiceName());
            if(mergeRequest == null)
                return;
            MergeRequest = Branch.MergeRequest;
            UpdateMergeRequest();
        }

        string GetServiceName() {
            if(!PerformTesting)
                return IsServiceUser(Assignee) ? Author : Assignee;
            return Branch.TestServiceName;
        }
        static bool IsServiceUser(string assignee) {
            return !string.IsNullOrEmpty(assignee) && assignee.StartsWith("dxvcs2git.");
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
        void PerformTestingChanged() {
            IsModified = true;
        }
    }
}
