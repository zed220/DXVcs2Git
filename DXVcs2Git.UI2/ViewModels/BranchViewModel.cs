using DevExpress.Mvvm;
using DXVcs2Git.Git;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DXVcs2Git.UI2 {
    public class BranchViewModel : ViewModelBase {
        public readonly GitLabWrapper GitLabWrapper;

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

        public BranchViewModel(GitLabWrapper gitLabWrapper, RepositoryViewModel repository, string branch) {
            GitLabWrapper = gitLabWrapper;
            Repository = repository;
            Name = branch;
        }

        public async void RefreshMergeRequestAsync() {
            IsLoading = true;
            await Repository.Repositories.SelectBranch(this);
            IsLoading = false;
        }
    }
}
