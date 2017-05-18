using DevExpress.Mvvm;
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
        public bool SupportsTesting { get; }

        public BranchViewModel(GitLabWrapper gitLabWrapper, RepositoryViewModel repository, string branch) {
            GitLabWrapper = gitLabWrapper;
            Repository = repository;
            Name = branch;
            SupportsTesting = ServiceLocator.Current.GetInstance<IMainViewModel>().Config.SupportsTesting && Repository.RepoConfig.SupportsTesting;
        }

        public async void RefreshMergeRequestAsync() {
            if(TrySelectBranch())
                return;
            IsLoading = true;
            MergeRequest mergeRequest = null;
            await Task.Run(() => {
                mergeRequest = GitLabWrapper.GetMergeRequests(Repository.Upstream, x => x.SourceProjectId == Repository.Origin.Id && x.SourceBranch == Name).FirstOrDefault();
            });
            if(mergeRequest != null) {
                if(!TrySelectBranch()) {
                    MergeRequest = mergeRequest;
                    ModuleManager.DefaultManager.Register(Regions.MergeRequest, new Module(Repository.Name + Name, ServiceLocator.Current.GetInstance<IMergeRequestViewModel>, typeof(MergeRequestView)));
                    ModuleManager.DefaultManager.InjectOrNavigate(Regions.MergeRequest, Repository.Name + Name, new MergeParameter(this, mergeRequest));
                    SubscribeViewModelRemovedEvent();
                }
            }
            else {
                UnselectBranch(this);
            }
            IsLoading = false;
        }

        static bool eventsSubscribed = false;
        
        static void SubscribeViewModelRemovedEvent() {
            if(eventsSubscribed)
                return;
            eventsSubscribed = true;
            ModuleManager.DefaultManager.GetEvents(Regions.MergeRequest).ViewModelRemoved += RepositoriesViewModel_ViewModelRemoved;
        }

        static void RepositoriesViewModel_ViewModelRemoved(object sender, ViewModelRemovedEventArgs e) {
            MergeRequestViewModel vm = e.ViewModel as MergeRequestViewModel;
            UnselectBranch(vm.Branch);
        }

        bool TrySelectBranch() {
            if(ModuleManager.DefaultManager.GetModule(Regions.MergeRequest, Repository.Name + Name) != null) {
                ModuleManager.DefaultManager.Navigate(Regions.MergeRequest, Repository.Name + Name);
                return true;
            }
            return false;
        }

        public static void UnselectBranch(BranchViewModel branch) {
            if(ModuleManager.DefaultManager.GetModule(Regions.MergeRequest, branch.Repository.Name + branch.Name) != null)
                ModuleManager.DefaultManager.Unregister(Regions.MergeRequest, branch.Repository.Name + branch.Name);
            branch.MergeRequest = null;
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
