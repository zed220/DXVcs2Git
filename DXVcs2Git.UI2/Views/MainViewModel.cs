using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.ModuleInjection;
using DevExpress.Mvvm.UI;
using DXVcs2Git.Core.Configuration;
using Microsoft.Practices.ServiceLocation;
using DevExpress.Xpf.Core;
using System.Collections.Generic;

namespace DXVcs2Git.UI2{
    public interface IWorker {
        bool IsLoading { get; }
    }
    public interface IMainViewModel {
        Config Config { get; }
        void WorkStarted(IWorker worker);
        void WorkFinished(IWorker worker);
    }

    public class MainViewModel : ViewModelBase, IMainViewModel {
        public Config Config { get; private set; }

        List<IWorker> Workers { get; } = new List<IWorker>();

        public ScrollBarMode ScrollBarMode {
            get { return GetProperty(() => ScrollBarMode); }
            set { SetProperty(() => ScrollBarMode, value); }
        }
        public bool IsLoading {
            get { return GetProperty(() => IsLoading); }
            set { SetProperty(() => IsLoading, value); }
        }

        public MainViewModel() {
            UpdateConfig();
        }

        public void ShowSettings() {
            ModuleManager.DefaultWindowManager.Show(Regions.Settings, Modules.SettingsView);
            ModuleManager.DefaultWindowManager.Clear(Regions.Settings);
            UpdateConfig();
        }

        void UpdateConfig() {
            var config = ConfigSerializer.GetConfig();
            if(Config != null && ConfigSerializer.IsConfigEquals(config, Config))
                return;
            Config = config;
            UpdateAppearance();
            UpdateContent();
        }

        void UpdateContent() {
            ModuleManager.DefaultManager.Clear(Regions.Content);
            ModuleManager.DefaultManager.InjectOrNavigate(Regions.Content, Modules.RepositoriesViewContent);
        }

        void UpdateAppearance() {
            ScrollBarMode = (ScrollBarMode)Config.ScrollBarMode;
            ApplicationThemeHelper.ApplicationThemeName = Config?.DefaultTheme ?? Theme.DefaultThemeName;
            NativeMethods.HotKeyHelper.UnregisterHotKey();
            NativeMethods.HotKeyHelper.RegisterHotKey(Config.KeyGesture);
        }

        public void WorkStarted(IWorker worker) {
            lock(this) {
                if(Workers.Contains(worker))
                    throw new NotSupportedException("Try add already existing worker");
                Workers.Add(worker);
                IsLoading = Workers.Count > 0;
            }
        }

        public void WorkFinished(IWorker worker) {
            lock(this) {
                if(!Workers.Contains(worker))
                    throw new NotSupportedException("Try remove missing worker");
                Workers.Remove(worker);
                IsLoading = Workers.Count > 0;
            }
        }
    }
}