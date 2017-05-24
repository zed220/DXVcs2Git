using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using DXVcs2Git.Core;
using DXVcs2Git.Core.Configuration;
using DXVcs2Git.Core.Git;
using NGitLab.Models;
using DevExpress.Mvvm.ModuleInjection;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Specialized;

namespace DXVcs2Git.UI2 {
    public interface IRepositoriesViewModel {
    }

    public class RepositoriesViewModel : ViewModelBase, IRepositoriesViewModel {
        public bool IsLoading {
            get { return GetProperty(() => IsLoading); }
            set { SetProperty(() => IsLoading, value); }
        }

        public ObservableCollection<RepositoryViewModel> Repositories {
            get { return GetProperty(() => Repositories); }
            set { SetProperty(() => Repositories, value); }
        }
        public RepoConfigsReader RepoConfigs {
            get { return GetProperty(() => RepoConfigs); }
            set { SetProperty(() => RepoConfigs, value); }
        }
        public ViewModelBase SelectedItem {
            get { return GetProperty(() => SelectedItem); }
            set { SetProperty(() => SelectedItem, value, OnSelectedItemChanged); }
        }
        public RepositoryViewModel SelectedRepository { get { return SelectedItem as RepositoryViewModel; } }
        public BranchViewModel SelectedBranch { get { return SelectedItem as BranchViewModel; } }

        readonly IMainViewModel mainViewModel;

        public RepositoriesViewModel(IMainViewModel mainViewModel) {
            this.mainViewModel = mainViewModel;
            ModuleManager.DefaultManager.GetEvents(this).Navigated += RepositoriesViewModel_Navigated;
            ModuleManager.DefaultManager.GetEvents(this).NavigatedAway += RepositoriesViewModel_NavigatedAway;
            
        }

        void RepositoriesViewModel_NavigatedAway(object sender, NavigationEventArgs e) {
            SelectedItem = null;
            foreach(var repo in Repositories)
                repo.CleanBranches(UnselectBranch);
            Repositories.Clear();
        }

        void RepositoriesViewModel_Navigated(object sender, NavigationEventArgs e) {
            LoadRepositoriesAsync();
        }

        async void LoadRepositoriesAsync() {
            IsLoading = true;
            await Task.Run(() => {
                RepoConfigs = new RepoConfigsReader();
                Repositories = new ObservableCollection<RepositoryViewModel>();
                List<Task> loadBranchesTaskList = new List<Task>();
                foreach(var repo in mainViewModel.Config.Repositories.With(x => x.Where(IsValidConfig).Select(repo => new RepositoryViewModel(repo.Name, repo, RepoConfigs[repo.ConfigName])))) {
                    Repositories.Add(repo);
                    loadBranchesTaskList.Add(Task.Run(new Action(() => repo.LoadBranches(UnselectBranch))));
                }
                Task.WaitAll(loadBranchesTaskList.ToArray());
            });
            IsLoading = false;
        }

        bool IsValidConfig(TrackRepository repo) {
            if(string.IsNullOrEmpty(repo.Name))
                return false;
            if(!RepoConfigs.HasConfig(repo.ConfigName))
                return false;
            if(string.IsNullOrEmpty(repo.LocalPath))
                return false;
            if(string.IsNullOrEmpty(repo.Server))
                return false;
            if(string.IsNullOrEmpty(repo.Token))
                return false;
            return DirectoryHelper.IsGitDir(repo.LocalPath);
        }

        async void OnSelectedItemChanged() {
            BranchViewModel branch = SelectedBranch;
            if(branch == null)
                return;
            if(branch.MergeRequest != null) {
                SelectBranch(branch);
                return;
            }
            if(branch.IsLoading)
                return;
            await Task.Run(() => branch.RefreshMergeRequest());
            if(branch.MergeRequest == null) {
                UnselectBranch(branch);
                return;
            }
            SelectBranch(branch);
        }

        static string GetBranchModuleName(BranchViewModel branch) {
            return branch.Repository.Name + branch.Name;
        }

        void RepositoriesViewModel_ViewModelRemoved(object sender, ViewModelRemovedEventArgs e) {
            IMergeRequestViewModel mergeRequest = (IMergeRequestViewModel)e.ViewModel;
            ModuleManager.DefaultManager.GetEvents(mergeRequest).ViewModelRemoved -= RepositoriesViewModel_ViewModelRemoved;
            UnselectBranch(mergeRequest.Branch);
        }

        void SelectBranch(BranchViewModel branch) {
            string moduleName = GetBranchModuleName(branch);
            if(ModuleManager.DefaultManager.GetModule(Regions.MergeRequest, moduleName) == null) {
                ModuleManager.DefaultManager.Register(Regions.MergeRequest, new Module(moduleName, () => {
                    IMergeRequestViewModel mergeRequest = ServiceLocator.Current.GetInstance<IMergeRequestViewModel>();
                    ModuleManager.DefaultManager.GetEvents(mergeRequest).ViewModelRemoved += RepositoriesViewModel_ViewModelRemoved;
                    return mergeRequest;
                }, typeof(MergeRequestView)));
            }
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.MergeRequest, moduleName, branch);
        }
        void UnselectBranch(BranchViewModel branch) {
            if(branch.MergeRequest == null)
                return;
            branch.MergeRequest = null;
            string moduleName = GetBranchModuleName(branch);
            if(ModuleManager.DefaultManager.GetModule(Regions.MergeRequest, moduleName) != null) {
                ModuleManager.DefaultManager.Unregister(Regions.MergeRequest, moduleName);
            }
            return;
        }
    }
}
