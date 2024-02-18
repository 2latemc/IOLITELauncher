using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using IoLiteLauncher.Backend;

namespace IoLiteLauncher {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private Instance? _instance;
        private void App_OnStartup(object sender, StartupEventArgs e) {
            _instance = Instance.Get;
            _instance.Init();


        }

        private void App_OnExit(object sender, ExitEventArgs e) {
            _instance.Shutdown();
        }
    }
}