using DevExpress.Mvvm;
using DevExpress.XtraEditors.DXErrorProvider;
using DXVcs2Git.Core.Configuration;
using DXVcs2Git.Core.Git;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXVcs2Git.UI2 {
    public class TrackRepositoryViewModel : BindableBase, IDataErrorInfo {
        readonly RepoConfigsReader ConfigsReader;
        public readonly TrackRepository Repo;

        TrackRepositoryViewModel(TrackRepository repo, RepoConfigsReader configsReader) {
            Repo = repo;
            ConfigsReader = configsReader;
            Name = repo.Name;
            ConfigName = repo.ConfigName;
            Token = repo.Token;
            LocalPath = repo.LocalPath;
            Server = repo.Server;
        }

        public string Name {
            get { return GetProperty(() => Name); }
            set { SetProperty(() => Name, value, UpdateName); }
        }
        public string ConfigName {
            get { return GetProperty(() => ConfigName); }
            set { SetProperty(() => ConfigName, value, UpdateConfigName); }
        }
        public RepoConfig RepoConfig {
            get { return GetProperty(() => RepoConfig); }
            set { SetProperty(() => RepoConfig, value); }
        }
        public string Token {
            get { return GetProperty(() => Token); }
            set { SetProperty(() => Token, value, UpdateToken); }
        }
        public string LocalPath {
            get { return GetProperty(() => LocalPath); }
            set { SetProperty(() => LocalPath, value, UpdateLocalPath); }
        }
        public string Server {
            get { return GetProperty(() => Server); }
            set { SetProperty(() => Server, value, UpdateServer); }
        }
        public string Error {
            get { return GetProperty(() => Error); }
            set { SetProperty(() => Error, value); }
        }


        public static TrackRepositoryViewModel Create(TrackRepository repo, RepoConfigsReader configsReader) {
            return new TrackRepositoryViewModel(repo, configsReader);
        }

        public void AskLocalPath() {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = LocalPath;
            var result = dialog.ShowDialog();
            if(result == DialogResult.Cancel)
                return;
            LocalPath = dialog.SelectedPath;
        }

        public string this[string columnName] {
            get { return GetError(columnName); }
        }

        string GetError(string columnName) {
            switch(columnName) {
                case nameof(Name):
                case nameof(ConfigName):
                case nameof(Token):
                case nameof(LocalPath):
                case nameof(Server):
                    return GetPropertyError(columnName);
            }
            return null;
        }
        string GetPropertyError(string columnName) {
            String res = GetType().GetProperty(columnName).GetValue(this) as String;
            return String.IsNullOrEmpty(res) ? $"The {columnName} is Empty" : null;
        }

        void UpdateErrorState() {
            List<string> res = new List<string>();
            res.Add(GetPropertyError(nameof(Name)));
            res.Add(GetPropertyError(nameof(ConfigName)));
            res.Add(GetPropertyError(nameof(Token)));
            res.Add(GetPropertyError(nameof(LocalPath)));
            res.Add(GetPropertyError(nameof(Server)));
            StringBuilder sb = new StringBuilder();
            res.ForEach(err => { if(err != null) sb.AppendLine(err); });
            Error = sb.ToString();
        }
        void UpdateName() {
            Repo.Name = Name;
            UpdateErrorState();
        }
        void UpdateConfigName() {
            Repo.ConfigName = ConfigName;
            UpdateRepoConfig();
            UpdateErrorState();
        }
        void UpdateToken() {
            Repo.Token = Token;
            UpdateErrorState();
        }
        void UpdateLocalPath() {
            Repo.LocalPath = LocalPath;
            UpdateErrorState();
        }
        void UpdateServer() {
            Repo.Server = Server;
            UpdateErrorState();
        }
        void UpdateRepoConfig() {
            RepoConfig = ConfigName != null ? ConfigsReader[ConfigName] : null;
            Server = RepoConfig?.Server;
        }

        public override string ToString() {
            return Name;
        }
    }
}
