using DevExpress.Xpf.Core;
using DXVcs2Git.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DXVcs2Git.UI2 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            ApplicationThemeHelper.UseLegacyDefaultTheme = true;
            ConfigSerializer.Version = 1;
        }

        protected override void OnStartup(StartupEventArgs e) {
            Bootstrapper.Run();
        }
    }
}
