using DevExpress.Mvvm;
using DevExpress.Mvvm.ModuleInjection;
using NGitLab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DXVcs2Git.UI2 {
    public interface IMergeRequestViewModel {
        BranchViewModel Branch { get; }
    }

    public class MergeRequestViewModel : ViewModelBase, IMergeRequestViewModel {
        public MergeRequest MergeRequest { get; private set; }
        public BranchViewModel Branch { get; private set; }
        public string SourceBranch { get; private set; }
        public string TargetBranch { get; private set; }
        public string Author { get; private set; }
        public string Assignee { get; private set; }
        public bool SupportsTesting { get; private set; }
        protected override void OnParameterChanged(object parameter) {
            UpdateMergeRequest();
        }
        public bool IsModified {
            get { return GetProperty(() => IsModified); }
            private set { SetProperty(() => IsModified, value); }
        }
        public string Title {
            get { return GetProperty(() => Title); }
            set { SetProperty(() => Title, value, TitleChanged); }
        }
        public string Description {
            get { return GetProperty(() => Description); }
            set { SetProperty(() => Description, value, DescriptionChanged); }
        }

        public BranchViewModel MergeParameter { get { return Parameter as BranchViewModel; } }

        public MergeRequestViewModel() {
        }

        void UpdateMergeRequest() {
            Branch = MergeParameter;
            MergeRequest = Branch.MergeRequest;
            if(MergeRequest == null) {
                Title = null;
                SourceBranch = null;
                TargetBranch = null;
                Author = null;
                Assignee = null;
                SupportsTesting = false;
                return;
            }
            
            Title = MergeRequest.Title;
            Description = MergeRequest.Description;
            SourceBranch = MergeRequest.SourceBranch;
            TargetBranch = MergeRequest.TargetBranch;
            Author = MergeRequest.Author.Username;
            Assignee = MergeRequest.Assignee?.Username;
            SupportsTesting = Branch?.SupportsTesting ?? false;
        }

        public void Apply() {
            if(!IsModified)
                return;

        }

        void DescriptionChanged() {
            if(Description == MergeRequest.Description)
                return;
            IsModified = true;
        }
        void TitleChanged() {
            if(Title == MergeRequest.Title)
                return;
            IsModified = true;
        }
    }
}
