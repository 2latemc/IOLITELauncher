using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using IoLiteLauncher.Backend;
using IoliteLauncher.Backend.Core;
using System.Windows.Controls;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using IoLiteLauncher.Utils;

namespace IoliteLauncher.Views  {
    public partial class MainWindow : Window, INotifyPropertyChanged {

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private readonly Instance _instance;

        private ObservableCollection<ProjectsManager.ProjectData> _projectDatas;
        public ObservableCollection<ProjectsManager.ProjectData> Projects {
            get => _projectDatas;
            set {
                _projectDatas = value;
                OnPropertyChanged();
            }
        }

        public MainWindow() {
            DataContext = this;
            _instance = Instance.Get;

            RefreshProjects();

            InitializeComponent();

        }

        private void RefreshProjects() {
            var fetchProjects = _instance.ProjectsManager.FetchProjects();
            Projects = new ObservableCollection<ProjectsManager.ProjectData>();
            foreach (ProjectsManager.ProjectData project in fetchProjects) {
                Projects.Add(project);
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e) {
            SettingsWindow settingsWindow = new SettingsWindow(_instance.SettingsManager.SettingsData);
            settingsWindow.Show();
            Close();
        }

        private void OpenProject(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            var selected = (ProjectsManager.ProjectData) btn.DataContext;
            // if (selected == null) {
            //     MessageBox.Show("Select a project from the list fist");
            //     return;
            // }
            _instance.ProjectsManager.OpenProject(selected.Path);
        }

        private void BrowseProjectPathClicked(object sender, RoutedEventArgs e){
            Button btn = (Button)sender;
            var item = (ProjectsManager.ProjectData) btn.DataContext;
            Process.Start("explorer.exe", item.Path);

        }

        private void CreateProject(object sender, RoutedEventArgs e) {
            var newName = ProjectCreationName.Text;
            if (String.IsNullOrWhiteSpace(newName) || String.IsNullOrEmpty(newName)) {
                MessageBox.Show("Enter project name");
                return;
            }
            newName = newName.Replace(" ", "");

            _instance.ProjectsManager.CreateProject(new ProjectsManager.ProjectCreateOptions() {
                Name = newName,
                OpenAfterCreation = true,
                OrgName = "Example Org",
            });
            RefreshProjects();
        }

        private void SubmitBug(object sender, RoutedEventArgs e) {
            Downloader.OpenUrl(Statics.GithubIssuesUrl);
        }

        private void DeleteProject(object sender, RoutedEventArgs e) {
            Button btn = (Button)sender;
            var projData = (ProjectsManager.ProjectData) btn.DataContext;

            _instance.ProjectsManager.DeleteProject(projData);
            RefreshProjects();
        }

        private void RefreshBtn(object sender, RoutedEventArgs e) {
            RefreshProjects();
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}