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

namespace DXVcs2Git.UI2 {
    public class RepositoryViewModel : ViewModelBase {
        public string Name { get; }
        public Project Origin { get; }
        public Project Upstream { get; }
        GitLabWrapper GitLabWrapper { get; }
        GitReaderWrapper GitReader { get; }
        public TrackRepository TrackRepository { get; }
        public ObservableCollection<BranchViewModel> Branches {
            get { return GetProperty(() => Branches); }
            set { SetProperty(() => Branches, value); }
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
            LoadBranches();
        }

        void LoadBranches() {
            if(Origin == null) {
                //Log.Error("Can`t find project");
                return;
            }

            var branches = this.GitLabWrapper.GetBranches(Origin).ToList();
            var localBranches = GitReader.GetLocalBranches();
            Branches = new ObservableCollection<BranchViewModel>(branches.Where(x => !x.Protected && localBranches.Any(local => local.FriendlyName == x.Name))
                .Select(x => new BranchViewModel(GitLabWrapper, this, x.Name)));

        }
    }
}
