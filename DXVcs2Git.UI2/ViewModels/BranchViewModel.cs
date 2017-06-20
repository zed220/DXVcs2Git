using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Layout.Core;
using DXVcs2Git.Core.Git;
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
            set { SetProperty(() => MergeRequest, value, UpdateCommands); }
        }
        public bool CanCreateMergeRequest {
            get { return GetProperty(() => CanCreateMergeRequest); }
            set { SetProperty(() => CanCreateMergeRequest, value); }
        }
        public bool CanCloseMergeRequest {
            get { return GetProperty(() => CanCloseMergeRequest); }
            set { SetProperty(() => CanCloseMergeRequest, value); }
        }
        public ICommand CreateMergeRequestCommand { get; private set; }
        public ICommand CloseMergeRequestCommand { get; private set; }

        public string TestServiceName;
        public bool SupportsTesting { get; }

        string GetBranchModuleName() {
            return Repository.Name + Name;
        }

        public void ShowMergeRequest() {
            string moduleName = GetBranchModuleName();
            if(ModuleManager.DefaultManager.GetModule(Regions.MergeRequest, moduleName) == null) {
                ModuleManager.DefaultManager.Register(Regions.MergeRequest, new Module(moduleName, () => {
                    IMergeRequestViewModel mergeRequest = ServiceLocator.Current.GetInstance<IMergeRequestViewModel>();
                    ModuleManager.DefaultManager.GetEvents(mergeRequest).ViewModelRemoved += RepositoriesViewModel_ViewModelRemoved;
                    return mergeRequest;
                }, typeof(MergeRequestView)));
            }
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.MergeRequest, moduleName, this);
        }
        public void HideMergeRequest() {
            if(MergeRequest == null)
                return;
            string moduleName = GetBranchModuleName();
            if(ModuleManager.DefaultManager.GetModule(Regions.MergeRequest, moduleName) != null) {
                ModuleManager.DefaultManager.Unregister(Regions.MergeRequest, moduleName);
            }
        }

        void RepositoriesViewModel_ViewModelRemoved(object sender, ViewModelRemovedEventArgs e) {
            IMergeRequestViewModel mergeRequest = (IMergeRequestViewModel)e.ViewModel;
            ModuleManager.DefaultManager.GetEvents(mergeRequest).ViewModelRemoved -= RepositoriesViewModel_ViewModelRemoved;
        }

        public BranchViewModel(GitLabWrapper gitLabWrapper, RepositoryViewModel repository, string branch) : base(ServiceLocator.Current.GetInstance<IMainViewModel>()) {
            GitLabWrapper = gitLabWrapper;
            Repository = repository;
            Name = branch;
            SupportsTesting = MainViewModel.Config.SupportsTesting && Repository.RepoConfig.SupportsTesting;
            TestServiceName = Repository.RepoConfig.TestServiceName ?? Repository.RepoConfig.DefaultServiceName;
            CreateCommands();
        }

        void CreateCommands() {
            CreateMergeRequestCommand = DelegateCommandFactory.Create(CreateMergeRequest, CanCreateMergeRequest);
            CloseMergeRequestCommand = DelegateCommandFactory.Create(CloseMergeRequest, CanCloseMergeRequest);
        }

        void CreateMergeRequest() {
            var branchInfo = GitLabWrapper.GetBranch(Repository.Origin, Name);
            string message = branchInfo.Commit.Message;
            string title = CalcMergeRequestTitle(message);
            string description = CalcMergeRequestDescription(message);
            string targetBranch = CalcTargetBranch();
            if(targetBranch == null) {
                //TODO: messagebox error
                return;
            }
            MergeRequest = GitLabWrapper.CreateMergeRequest(Repository.Origin, Repository.Upstream, title, description, null, Name, targetBranch);
            ShowMergeRequest();
        }
        public MergeRequest UpdateMergeRequest(string title, string description, string assignee) {
            MergeRequest mergeRequest = null;
            if(MergeRequest.Title != title || MergeRequest.Description != description)
                mergeRequest = GitLabWrapper.UpdateMergeRequestTitleAndDescription(mergeRequest ?? MergeRequest, title, description);
            if(MergeRequest.Assignee?.Name != assignee)
                mergeRequest = GitLabWrapper.UpdateMergeRequestAssignee(mergeRequest ?? MergeRequest, assignee);
            if(mergeRequest == null)
                return null;
            MergeRequest = mergeRequest;
            return MergeRequest;
        }

        string CalcTargetBranch() {
            RepoConfig repoConfig = Repository.RepoConfig;
            return repoConfig?.TargetBranch;
        }
        static string CalcMergeRequestDescription(string message) {
            if(string.IsNullOrEmpty(message))
                return string.Empty;
            var changes = message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            changes.Skip(1).ForEach(x => sb.AppendLine(x.ToString()));
            return sb.ToString();
        }
        static string CalcMergeRequestTitle(string message) {
            if(string.IsNullOrEmpty(message))
                return string.Empty;
            var changes = message.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var title = changes.FirstOrDefault();
            return title;
        }

        void UpdateCommands() {
            CanCreateMergeRequest = MergeRequest == null && !IsLoading;
            CanCloseMergeRequest = MergeRequest != null && !IsLoading;
        }
        protected override void OnLoadingChanged() {
            base.OnLoadingChanged();
            UpdateCommands();
        }

        void CloseMergeRequest() {
            GitLabWrapper.CloseMergeRequest(MergeRequest);
            HideMergeRequest();
            MergeRequest = null;
        }

        public void RefreshMergeRequest() {
            if(MergeRequest != null)
                return;
            IsLoading = true;
            MergeRequest = GitLabWrapper.GetMergeRequests(Repository.Upstream, x => x.SourceProjectId == Repository.Origin.Id && x.SourceBranch == Name).FirstOrDefault();
            IsLoading = false;
        }
        public async Task<IEnumerable<Commit>> GetCommits() {
            return await Task.Run(() => GitLabWrapper.GetMergeRequestCommits(MergeRequest));
        }
        public async Task<IEnumerable<MergeRequestFileData>> GetMergeRequestChanges() {
            return await Task.Run(() => GitLabWrapper.GetMergeRequestChanges(MergeRequest));
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
