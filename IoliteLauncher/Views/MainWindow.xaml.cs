using System.Windows;
using IoLiteLauncher.Backend;

namespace IoliteLauncher.Views {
    public partial class MainWindow : Window {
        private Instance _instance;
        public MainWindow() {
            _instance = Instance.Get;
            InitializeComponent();
        }

        private void OpenSettings(object sender, RoutedEventArgs e) {
            SettingsWindow settingsWindow = new SettingsWindow(_instance.SettingsManager.SettingsData);
            settingsWindow.Show();
            Close();
        }

        private void OpenProject(object sender, RoutedEventArgs e) {
            _instance.ProjectsManager.OpenProject("");
        }
    }
}