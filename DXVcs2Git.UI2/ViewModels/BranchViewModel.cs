using DevExpress.Mvvm;
using DXVcs2Git.Git;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace DXVcs2Git.UI2 {
    public class BranchViewModel : ViewModelBase {
        readonly GitLabWrapper GitLabWrapper;

        public string Name { get; }
        public RepositoryViewModel Repository { get; }

        public MergeRequest MergeRequest {
            get { return GetProperty(() => MergeRequest); }
            set { SetProperty(() => MergeRequest, value); }
        }
        public bool IsLoading {
            get { return GetProperty(() => IsLoading); }
            private set { SetProperty(() => IsLoading, value); }
        }

        public ICommand HideCommand { get; }

        public BranchViewModel(GitLabWrapper gitLabWrapper, RepositoryViewModel repository, string branch) {
            HideCommand = new DelegateCommand(Hide);
            GitLabWrapper = gitLabWrapper;
            Repository = repository;
            Name = branch;
        }

        public async void RefreshMergeRequestAsync() {
            IsLoading = true;
            await Repository.Repositories.SelectBranch(this);
            IsLoading = false;
        }

        void Hide() {
            Repository.Repositories.UnselectBranch(this);
        }

        protected bool Equals(BranchViewModel other) {
            return this.Repository.Equals(other.Repository) && Name == other.Name;
        }
        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj))
                return false;
            if(ReferenceEquals(this, obj))
                return true;
            if(obj.GetType() != this.GetType())
                return false;
            return Equals((BranchViewModel)obj);
        }
        public override int GetHashCode() {
            return 0;
        }
    }
}
