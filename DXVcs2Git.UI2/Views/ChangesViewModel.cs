using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public interface IChangesViewModel : ISupportParameter, IWorker { }

    public class ChangesViewModel : ViewModelWorkerBase, IChangesViewModel {
        public ChangesViewModel(IMainViewModel mainViewModel) : base(mainViewModel) { }

        public ObservableCollection<MergeRequestFileDataViewModel> Changes {
            get { return GetProperty(() => Changes); }
            private set { SetProperty(() => Changes, value); }
        }
        public BranchViewModel Branch { get { return Parameter as BranchViewModel; } }

        protected override void OnParameterChanged(object parameter) {
            if(!StartLoading())
                return;
            Task.Run(() => UpdateChanges());
        }

        async void UpdateChanges() {
            if(Branch == null) {
                Changes = new ObservableCollection<MergeRequestFileDataViewModel>();
                return;
            }
            var changes = new ObservableCollection<MergeRequestFileDataViewModel>();
            (await Branch.GetMergeRequestChanges()).ToList().ForEach(c => changes.Add(new MergeRequestFileDataViewModel(c)));
            Changes = changes;
            EndLoading();
        }
    }
}
