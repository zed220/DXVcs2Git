using DevExpress.Mvvm;
using DXVcs2Git.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXVcs2Git.UI2 {
    public class RepositoryViewModel : ViewModelBase {
        public RepositoryViewModel(string name, TrackRepository trackRepository, RepositoriesViewModel repositories) {
            Name = name;
        }

        public string Name {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value); }
        }

    }
}
