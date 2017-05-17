using DevExpress.Mvvm;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DXVcs2Git.UI2 {
    public interface IMergeRequestViewModel { }

    public class MergeRequestViewModel : BindableBase, IMergeRequestViewModel, ISupportParameter {
        public MergeRequest MergeRequest { get; private set; }
        public BranchViewModel Branch { get; private set; }
        public string Title { get; private set; }
        public string SourceBranch { get; private set; }
        public string TargetBranch { get; private set; }
        public string Author { get; private set; }
        public string Assignee { get; private set; }
        public object Parameter {
            get { return GetProperty(() => Parameter); }
            set { SetProperty(() => Parameter, value, UpdateMergeRequest); }
        }
        public MergeParameter MergeParameter { get { return Parameter as MergeParameter; } }

        public MergeRequestViewModel() {
        }

        void UpdateMergeRequest() {
            Branch = MergeParameter.Item1;
            MergeRequest = MergeParameter.Item2;
            if(MergeRequest == null) {
                Title = null;
                SourceBranch = null;
                TargetBranch = null;
                Author = null;
                Assignee = null;
                return;
            }
            Title = MergeRequest.Title;
            SourceBranch = MergeRequest.SourceBranch;
            TargetBranch = MergeRequest.TargetBranch;
            Author = MergeRequest.Author.Username;
            Assignee = MergeRequest.Assignee?.Username;
        }
    }
}
