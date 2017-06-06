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

    public class RepositoriesViewModel :  ViewModelWorkerBase, IRepositoriesViewModel {
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
                repo.CleanBranches(b => b.HideMergeRequest());
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
                    loadBranchesTaskList.Add(Task.Run(new Action(() => repo.LoadBranches(b => b.HideMergeRequest()))));
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
                branch.ShowMergeRequest();
                return;
            }
            if(branch.IsLoading)
                return;
            await Task.Run(() => branch.RefreshMergeRequest());
            if(branch.MergeRequest == null) {
                branch.HideMergeRequest();
                return;
            }
            branch.ShowMergeRequest();
        }
    }
}
