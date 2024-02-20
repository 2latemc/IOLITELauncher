using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using IoLiteLauncher.Backend;
using IoLiteLauncher.Utils;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;

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
            if (_instance.ProjectsManager.EngineProcess.IsRunning()) {
                MessageBox.Show("Looks like Engine is still running, closing now!", "", MessageBoxButtons.OK);
                var process = _instance.ProjectsManager.EngineProcess;
                process.Kill();
                process.WaitForExit();
                process.Dispose();
            }
            _instance.Shutdown();
        }
    }
}