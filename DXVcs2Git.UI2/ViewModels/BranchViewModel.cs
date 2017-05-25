﻿using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Xpf.Layout.Core;
using DXVcs2Git.Git;
using Microsoft.Practices.ServiceLocation;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace DXVcs2Git.UI2 {
    public class BranchViewModel : ViewModelWorkerBase {
        readonly GitLabWrapper GitLabWrapper;

        public string Name { get; }
        public RepositoryViewModel Repository { get; }

        public MergeRequest MergeRequest {
            get { return GetProperty(() => MergeRequest); }
            set { SetProperty(() => MergeRequest, value); }
        }

        public bool SupportsTesting { get; }

        public BranchViewModel(GitLabWrapper gitLabWrapper, RepositoryViewModel repository, string branch) {
            GitLabWrapper = gitLabWrapper;
            Repository = repository;
            Name = branch;
            SupportsTesting = ServiceLocator.Current.GetInstance<IMainViewModel>().Config.SupportsTesting && Repository.RepoConfig.SupportsTesting;
        }

        public void RefreshMergeRequest() {
            if(MergeRequest != null)
                return;
            IsLoading = true;
            MergeRequest = GitLabWrapper.GetMergeRequests(Repository.Upstream, x => x.SourceProjectId == Repository.Origin.Id && x.SourceBranch == Name).FirstOrDefault();
            IsLoading = false;
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
