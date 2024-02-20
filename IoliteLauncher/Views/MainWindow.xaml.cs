using System.Collections.ObjectModel;
using System.Windows;
using IoLiteLauncher.Backend;
using IoliteLauncher.Backend.Core;
using Microsoft.VisualBasic.CompilerServices;
using System.Windows.Controls;
using System.Diagnostics;
using IoLiteLauncher.Utils;

namespace IoliteLauncher.Views  {
    public partial class MainWindow : Window {
        private Instance _instance;

        public ObservableCollection<ProjectsManager.ProjectData> Projects { get; set; }
        public MainWindow() {
            DataContext = this;
            _instance = Instance.Get;

            var fetchProjects = _instance.ProjectsManager.FetchProjects();
            Projects = new ObservableCollection<ProjectsManager.ProjectData>();
            foreach (ProjectsManager.ProjectData project in fetchProjects) {
                Projects.Add(project);
            }

            InitializeComponent();

        }

        private void OpenSettings(object sender, RoutedEventArgs e) {
            SettingsWindow settingsWindow = new SettingsWindow(_instance.SettingsManager.SettingsData);
            settingsWindow.Show();
            Close();
        }

        private void OpenProject(object sender, RoutedEventArgs e) {
            ProjectsManager.ProjectData? selected = (ProjectsManager.ProjectData?)ProjectsList.SelectedItem;
            if (selected == null) {
                MessageBox.Show("Select a project from the list first.");
                return;
            }
            _instance.ProjectsManager.OpenProject(selected.GetValueOrDefault().Path);
        }

        private void BrowseProjectPathClicked(object sender, RoutedEventArgs e){
            Button btn = (Button)sender;
            var item = (ProjectsManager.ProjectData) btn.DataContext;
            Process.Start("explorer.exe", item.Path);

        }

        private void CreateProject(object sender, RoutedEventArgs e) {
            _instance.ProjectsManager.StartEngine();
        }

        private void SubmitBug(object sender, RoutedEventArgs e) {
            Downloader.OpenUrl(Statics.GithubIssuesURL);
        }
    }
}