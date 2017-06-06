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
using System.Threading;
using System;
using DevExpress.Xpf.Layout.Core;
using Microsoft.Practices.ServiceLocation;

namespace DXVcs2Git.UI2 {
    public class RepositoryViewModel : ViewModelWorkerBase {
        public string Name { get; }
        public Project Origin { get; }
        public Project Upstream { get; }
        public GitLabWrapper GitLabWrapper { get; }
        GitReaderWrapper GitReader { get; }
        public TrackRepository TrackRepository { get; }
        public RepoConfig RepoConfig { get; private set; }
        public ObservableCollection<BranchViewModel> Branches {
            get { return GetProperty(() => Branches); }
            set { SetProperty(() => Branches, value); }
        }

        public ICommand CreateMergeRequestCommand { get; private set; }
        public ICommand CloseMergeRequestCommand { get; private set; }

        public RepositoryViewModel(string name, TrackRepository trackRepository, RepoConfig repoConfig) {
            Name = name;
            TrackRepository = trackRepository;
            RepoConfig = repoConfig;
            GitLabWrapper = new GitLabWrapper(TrackRepository.Server, TrackRepository.Token);
            GitReader = new GitReaderWrapper(trackRepository.LocalPath);
            Origin = GitLabWrapper.FindProject(GitReader.GetOriginRepoPath());
            Upstream = GitLabWrapper.FindProject(GitReader.GetUpstreamRepoPath());
            Branches = new ObservableCollection<BranchViewModel>();
            CreateCommands();
        }

        void CreateCommands() {
            CreateMergeRequestCommand = DelegateCommandFactory.Create(Empty, CanExecute);
            CloseMergeRequestCommand = DelegateCommandFactory.Create(Empty, CanExecute);
        }

        void Empty() {

        }
        bool CanExecute() {
            return false;
        }

        public void LoadBranches(Action<BranchViewModel> cleanBranchAction) {
            if(Origin == null) {
                //Log.Error("Can`t find project");
                return;
            }
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
                cleanBranchAction?.Invoke(branch);
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

        public void CleanBranches(Action<BranchViewModel> cleanBranchAction) {
            foreach(var branch in Branches)
                cleanBranchAction?.Invoke(branch);
        }
    }
}
