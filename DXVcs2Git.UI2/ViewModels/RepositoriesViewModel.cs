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
        //public RepositoriesViewModel SelectedRepository {
        //    get { return GetProperty(() => SelectedRepository); }
        //    set { SetProperty(() => SelectedRepository, value); }
        //}
        public ObservableCollection<BranchViewModel> SelectedBranches {
            get { return GetProperty(() => SelectedBranches); }
            set { SetProperty(() => SelectedBranches, value); }
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
                SelectedBranches = new ObservableCollection<BranchViewModel>();
                Repositories = new ObservableCollection<RepositoryViewModel>(mainViewModel.Config.Repositories.With(x => x.Where(IsValidConfig).Select(repo => new RepositoryViewModel(repo.Name, repo, this))));
                IsLoading = false;
            });
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

        public async Task SelectBranch(BranchViewModel branch) {
            MergeRequest mergeRequest = null;
            await Task.Run(() => {
                mergeRequest = branch.GitLabWrapper.GetMergeRequests(branch.Repository.Upstream, x => x.SourceProjectId == branch.Repository.Origin.Id && x.SourceBranch == branch.Name).FirstOrDefault();
            });
            if(mergeRequest != null) {
                branch.MergeRequest = mergeRequest;
                //MergeRequest = new MergeRequestViewModel(this, mergeRequest);
                SelectedBranches.Add(branch);
                //ModuleManager.DefaultManager.Clear(Regions.MergeRequest);
                ModuleManager.DefaultManager.Register(Regions.MergeRequest + mergeRequest.Id, new Module(Modules.MergeRequestView, ServiceLocator.Current.GetInstance<IMergeRequestViewModel>, typeof(MergeRequestView)));
                ModuleManager.DefaultManager.InjectOrNavigate(Regions.MergeRequest + mergeRequest.Id, Modules.MergeRequestView, new MergeParameter(branch, mergeRequest));
            }
            else {
                if(SelectedBranches.Contains(branch)) {
                    ModuleManager.DefaultManager.Unregister(Regions.MergeRequest + mergeRequest.Id, Modules.MergeRequestView);
                    SelectedBranches.Remove(branch);
                    //ModuleManager.DefaultManager.InjectOrNavigate(Regions.MergeRequest, Modules.MergeRequestView, mergeRequest);
                    branch.MergeRequest = null;
                }
                //MergeRequest = null;
            }
        }
    }

    public class MergeParameter : Tuple<BranchViewModel, MergeRequest> {
        public MergeParameter(BranchViewModel item1, MergeRequest item2) : base(item1, item2) {
        }
    }
}
