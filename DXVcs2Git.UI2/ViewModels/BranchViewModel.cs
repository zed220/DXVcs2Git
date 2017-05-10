using DevExpress.Mvvm;
using DXVcs2Git.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public class BranchViewModel : ViewModelBase {
        public string Name { get; }

        public BranchViewModel(GitLabWrapper gitLabWrapper, RepositoryViewModel repository, string branch) {
            Name = branch;
        }
    }
}
