using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DXVcs2Git.Core;
using DXVcs2Git.Core.Configuration;
using DXVcs2Git.Core.Git;
using DXVcs2Git.Git;
using NGitLab.Models;
using System.Collections.ObjectModel;
using LibGit2Sharp;
using System.Windows.Input;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public class RepositoryViewModel : ViewModelBase {
        public string Name { get; }
        public Project Origin { get; }
        public Project Upstream { get; }
        public GitLabWrapper GitLabWrapper { get; }
        GitReaderWrapper GitReader { get; }
        public TrackRepository TrackRepository { get; }
        public ObservableCollection<BranchViewModel> Branches {
            get { return GetProperty(() => Branches); }
            set { SetProperty(() => Branches, value); }
        }
        public bool IsLoading {
            get { return GetProperty(() => IsLoading); }
            set { SetProperty(() => IsLoading, value); }
        }

        public RepositoriesViewModel Repositories { get; }

        public RepositoryViewModel(string name, TrackRepository trackRepository, RepositoriesViewModel repositories) {
            Repositories = repositories;
            Name = name;
            TrackRepository = trackRepository;
            GitLabWrapper = new GitLabWrapper(TrackRepository.Server, TrackRepository.Token);
            GitReader = new GitReaderWrapper(trackRepository.LocalPath);
            Origin = GitLabWrapper.FindProject(GitReader.GetOriginRepoPath());
            Upstream = GitLabWrapper.FindProject(GitReader.GetUpstreamRepoPath());
            Branches = new ObservableCollection<BranchViewModel>();
            //LoadBranchesAsync();
        }

        public async void LoadBranchesAsync() {
            if(Origin == null) {
                //Log.Error("Can`t find project");
                return;
            }
            await Task.Run(() => {
                
                LoadBranches();
                
            });
        }
        public void LoadBranches() {
            IsLoading = true;
            var branches = this.GitLabWrapper.GetBranches(Origin).ToList();
            var localBranches = GitReader.GetLocalBranches();
            var branchesVms = new ObservableCollection<BranchViewModel>(branches.Where(x => !x.Protected && localBranches.Any(local => local.FriendlyName == x.Name))
                .Select(x => new BranchViewModel(GitLabWrapper, this, x.Name)));
            List<BranchViewModel> addedBranches = new List<BranchViewModel>();
            List<BranchViewModel> removedBranches = new List<BranchViewModel>();
            foreach(var branch in Branches) {
                if(!branchesVms.Contains(branch))
                    removedBranches.Add(branch);
            }
            foreach(var branch in removedBranches) {
                Branches.Remove(branch);
                continue;
            }
            foreach(var branch in branchesVms) {
                if(!Branches.Contains(branch))
                    addedBranches.Add(branch);
            }
            foreach(var branch in addedBranches)
                Branches.Add(branch);
            IsLoading = false;
        }
    }
}
