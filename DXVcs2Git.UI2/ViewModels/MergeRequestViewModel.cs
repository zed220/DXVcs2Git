using DevExpress.Mvvm;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public interface IMergeRequestViewModel { }

    public class MergeRequestViewModel : BindableBase, IMergeRequestViewModel, ISupportParameter {
        public MergeRequest MergeRequest { get; private set; }
        public BranchViewModel Branch { get; private set; }
        public string Title { get; private set; }
        public string SourceBranch { get; private set; }
        public string TargetBranch { get; private set; }
        public object Parameter {
            get { return GetProperty(() => Parameter); }
            set { SetProperty(() => Parameter, value, UpdateMergeRequest); }
        }
        public MergeParameter MergeParameter { get { return Parameter as MergeParameter; } }

        public MergeRequestViewModel() {
            //BranchViewModel branch, MergeRequest mergeRequest
            //Branch = branch;
            //MergeRequest = mergeRequest;
            //Title = MergeRequest.Title;
            //SourceBranch = MergeRequest.SourceBranch;
            //TargetBranch = MergeRequest.TargetBranch;
        }

        void UpdateMergeRequest() {
            Branch = MergeParameter.Item1;
            MergeRequest = MergeParameter.Item2;
            Title = MergeRequest.Title;
            SourceBranch = MergeRequest.SourceBranch;
            TargetBranch = MergeRequest.TargetBranch;
        }
    }
}
