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
        private Instance _instance;

        private void App_OnStartup(object sender, StartupEventArgs e) {
            _instance = Instance.Get;
            _instance.Init();
        }

        private void App_OnExit(object sender, ExitEventArgs e) {
            if (_instance.ProjectsManager.EngineProcess.IsRunning()) {
                MessageBox.Show("Looks like Engine is still running, closing now!", "", MessageBoxButtons.OK);
                var process = _instance.ProjectsManager.EngineProcess;
                if (process != null) {
                    process.Kill();
                    process.WaitForExit();
                    process.Dispose();
                }
            }

            _instance.Shutdown();
        }
    }
}