using DevExpress.Mvvm;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.Practices.ServiceLocation;

namespace DXVcs2Git.UI2 {
    public class CommitViewModel : ViewModelWorkerBase {
        readonly Commit Commit;

        public CommitViewModel(Commit commit) : base(ServiceLocator.Current.GetInstance<IMainViewModel>()) {
            Commit = commit;
        }

        [Display(Order = 0)]
        public BuildStatus BuildStatus {
            get { return GetProperty(() => BuildStatus); }
            private set { SetProperty(() => BuildStatus, value); }
        }
        [Display(Order = 1)]
        public string Title {
            get { return GetProperty(() => Title); }
            private set { SetProperty(() => Title, value); }
        }
        [Display(Order = 2)]
        public string Duration {
            get { return GetProperty(() => Duration); }
            private set { SetProperty(() => Duration, value); }
        }
        [Display(AutoGenerateField = false)]
        public override bool IsLoading { get => base.IsLoading; }

        public void Update(BranchViewModel branch) {
            if(!StartLoading())
                return;
            Title = Commit.Title;
            var builds = branch.GetBuilds(Commit.Id);
            var build = builds.FirstOrDefault();
            if(build != null) {
                BuildStatus = build.Status ?? BuildStatus.undefined;
                Duration = GetDuration(build, BuildStatus);
            }
            EndLoading();
        }

        static string GetDuration(Build build, BuildStatus buildStatus) {
            if (build.StartedAt != null && (buildStatus == BuildStatus.success || buildStatus == BuildStatus.failed)) {
                return ((build.FinishedAt ?? DateTime.Now) - build.StartedAt.Value).ToString(@"hh\:mm\:ss");
            }
            else
                return string.Empty;
        }
    }
}
