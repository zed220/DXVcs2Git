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

        readonly IMainViewModel mainViewModel;

        public RepositoriesViewModel(IMainViewModel mainViewModel) {
            this.mainViewModel = mainViewModel;
            LoadRepositoriesAsync();
        }


        async void LoadRepositoriesAsync() {
            IsLoading = true;
            await Task.Run(() => {
                RepoConfigs = new RepoConfigsReader();
                Repositories = new ObservableCollection<RepositoryViewModel>();
                List<Task> loadBranchesTaskList = new List<Task>();
                foreach(var repo in mainViewModel.Config.Repositories.With(x => x.Where(IsValidConfig).Select(repo => new RepositoryViewModel(repo.Name, repo)))) {
                    Repositories.Add(repo);
                    loadBranchesTaskList.Add(Task.Run(new Action(repo.LoadBranches)));

                    //loadBranchesTaskList.Add(Task.Factory.StartNew(repo.LoadBranches));
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
    }

    public class MergeParameter : Tuple<BranchViewModel, MergeRequest> {
        public MergeParameter(BranchViewModel item1, MergeRequest item2) : base(item1, item2) {
        }
    }
}
