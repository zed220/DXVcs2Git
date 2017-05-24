using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXVcs2Git.Core.Configuration;
using DXVcs2Git.Core.Git;

namespace DXVcs2Git.UI2 {
    public interface IRepositoriesSettingsViewModel {
    }

    public class RepositoriesSettingsViewModel : ViewModelBase, IRepositoriesSettingsViewModel {
        public ObservableCollection<TrackRepositoryViewModel> Repositories { get; private set; } = new ObservableCollection<TrackRepositoryViewModel>();

        readonly Config Config;

        public RepoConfigsReader ConfigsReader { get; } = new RepoConfigsReader();

        public ObservableCollection<string> AvailableTokens {
            get { return GetProperty(() => AvailableTokens); }
            private set { SetProperty(() => AvailableTokens, value); }
        }
        public ObservableCollection<string> AvailableConfigs {
            get { return GetProperty(() => AvailableConfigs); }
            private set { SetProperty(() => AvailableConfigs, value); }
        }

        public RepositoriesSettingsViewModel(ISettingsViewModel settingsViewModel) {
            Config = settingsViewModel.Config;
            LoadRepositories();
            UpdateTokens();
        }

        void LoadRepositories() {
            if(Config.Repositories != null)
                Repositories = new ObservableCollection<TrackRepositoryViewModel>(Config.Repositories.Select(CreateRepositoryViewModel));
            Repositories.CollectionChanged += Repositories_CollectionChanged;
        }

        TrackRepositoryViewModel CreateRepositoryViewModel(TrackRepository repo) {
            return TrackRepositoryViewModel.Create(repo, ConfigsReader);
        }

        public void RemoveRepository(TrackRepositoryViewModel repository) {
            Repositories.Remove(repository);
            List<TrackRepository> reps = Config.Repositories.ToList();
            reps.Remove(repository.Repo);
            Config.Repositories = reps.ToArray();
        }
        public void AddNewRepository() {
            List<TrackRepository> reps = Config.Repositories.ToList();
            TrackRepository model = new TrackRepository();
            reps.Add(model);
            Config.Repositories = reps.ToArray();
            Repositories.Add(TrackRepositoryViewModel.Create(model, ConfigsReader));
        }

        void Repositories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            UpdateTokens();
        }

        void UpdateTokens() {
            AvailableTokens = new ObservableCollection<string>(Repositories.Select(repo => repo.Token).Distinct());
            var userConfigs = Repositories.Select(repo => repo.ConfigName).Distinct();
            AvailableConfigs = new ObservableCollection<string>(ConfigsReader.RegisteredConfigs.Select(config => config.Name).Except(userConfigs));
        }

        public override string ToString() {
            return "Repositories";
        }
    }
}
