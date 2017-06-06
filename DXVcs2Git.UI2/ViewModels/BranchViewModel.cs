using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Layout.Core;
using DXVcs2Git.Git;
using Microsoft.Practices.ServiceLocation;
using NGitLab;
using NGitLab.Models;
using System;
using System.Collections;
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
        public ICommand CreateMergeRequestCommand { get; private set; }
        public ICommand CloseMergeRequestCommand { get; private set; }

        public bool SupportsTesting { get; }

        public BranchViewModel(GitLabWrapper gitLabWrapper, RepositoryViewModel repository, string branch) {
            GitLabWrapper = gitLabWrapper;
            Repository = repository;
            Name = branch;
            SupportsTesting = ServiceLocator.Current.GetInstance<IMainViewModel>().Config.SupportsTesting && Repository.RepoConfig.SupportsTesting;
            CreateCommands();
        }

        void CreateCommands() {
            CreateMergeRequestCommand = DelegateCommandFactory.Create(CreateMergeRequest, CanCreateMergeRequest);
            CloseMergeRequestCommand = DelegateCommandFactory.Create(CloseMergeRequest, CanCloseMergeRequest);
        }

        void CreateMergeRequest() {

        }
        bool CanCreateMergeRequest() {
            return MergeRequest == null && !IsLoading;
        }
        void CloseMergeRequest() {

        }
        bool CanCloseMergeRequest() {
            return MergeRequest != null && !IsLoading;
        }

        public void RefreshMergeRequest() {
            if(MergeRequest != null)
                return;
            IsLoading = true;
            MergeRequest = GitLabWrapper.GetMergeRequests(Repository.Upstream, x => x.SourceProjectId == Repository.Origin.Id && x.SourceBranch == Name).FirstOrDefault();
            IsLoading = false;
        }
        public async Task<IEnumerable<Commit>> GetCommits() {
            IsLoading = true;
            var result = await Task.Run(() => GitLabWrapper.GetMergeRequestCommits(MergeRequest));
            IsLoading = false;
            return result;
        }
        public async Task<IEnumerable<MergeRequestFileData>> GetMergeRequestChanges() {
            IsLoading = true;
            var result = await Task.Run(() => GitLabWrapper.GetMergeRequestChanges(MergeRequest));
            IsLoading = false;
            return result;
        }
        public async Task<IEnumerable<Build>> GetBuilds(Sha1 sha) {
            return await Task.Run(() => GitLabWrapper.GetBuilds(MergeRequest, sha));
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
