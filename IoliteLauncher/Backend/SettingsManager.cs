using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using IoLiteLauncher.Utils;
using IoliteLauncher.Views;

namespace IoLiteLauncher.Backend;

public class SettingsManager {
    public SettingsData SettingsData = new SettingsData();

    private Instance _instance;

    public string ExecutablePath => Path.Combine(SettingsData.EnginePath, Statics.ExecutableName);

    private string _settingsPath = Path.Combine(Statics.AppDataPath, "settings.json");

    public SettingsManager() {
        _instance = Instance.Get;
    }

    public void Save() {
        bool success = Serializer.ToFile(_settingsPath, SettingsData);
        if (success) {
            Debug.WriteLine("Settings saved");
        }
        else {
            Debug.WriteLine("Could not save settings.");
        }
    }

    public void Load() {
        if (!File.Exists(_settingsPath)) {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
            return;
        }
        var data = Serializer.FromFile<SettingsData>(_settingsPath);
        if (data == null) {
            SettingsData = new SettingsData();
            SettingsData.ProjectsPaths = new List<string>
                { Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "IoliteProjects") };
        }
        else SettingsData = data;

        if (!IsValidEnginePath(SettingsData.EnginePath) || !IsValidProjectsPath()) {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
        else {
            StartMain();
        }
    }

    public void StartMain() {
        if (!File.Exists(_instance.ProjectsManager.ProjectDataStoragePath)) {
            _instance.ProjectsManager.MoveTemplateProjects();
        }
        else {
            var msgBoxResult = MessageBox.Show("Looks like there is a project currently open, would you like to close it now?", "",
                MessageBoxButton.YesNo);
            if (msgBoxResult == MessageBoxResult.Yes) {
                bool closed = _instance.ProjectsManager.CloseProject();
                if (!closed) {
                    MessageBox.Show("Could not close, aborting!");
                    Application.Current.Shutdown();
                    return;
                }
            }
            else {
                Application.Current.Shutdown();
                return;
            }
        }

        MainWindow mainWindow = new MainWindow();
        foreach (string path in SettingsData.ProjectsPaths) {
            try {
                string warningPath = Path.Combine(path, Statics.WarningFileName);
                if (!File.Exists(warningPath)) {
                    File.WriteAllText(warningPath,
                        "Never ever edit anything in here while the project is opened, your changes will be overriden! Only change anything in the .json files if you really know what you are doing.");
                }
            }
            catch {
                MessageBox.Show("couldnt create warning files");
            }
        }

        mainWindow.Show();
    }

    public bool IsValidProjectsPath(List<string>? projectsPathOverride = null) {
        List<string> projectsList;
        if (projectsPathOverride == null) {
            projectsList = SettingsData.ProjectsPaths;
        }
        else projectsList = projectsPathOverride;

        if (projectsList.Count <= 0) return false;

        try {
            foreach (var project in projectsList) {
                if (!Path.IsPathRooted(project)) return false;
                if (!Directory.Exists(project)) Directory.CreateDirectory(project);
            }

            return true;
        }
        catch (Exception e) {
            MessageBox.Show("Could not create default project directories " + e.Message);
            return false;
        }
    }

    public bool IsValidEnginePath(string path) {
        if (String.IsNullOrEmpty(path)) {
            return false;
        }

        if (!Directory.Exists(path)) {
            return false;
        }

        try {
            foreach (string file in Directory.GetFiles(path)) {
                if (Path.GetFileName(file).ToLower().Equals(Statics.ExecutableName.ToLower())) {
                    return true;
                }
            }
        }
        catch (Exception e) {
            return false;
        }

        return false;
    }
}

public class SettingsData {
    public List<string> ProjectsPaths = new List<string>() { };
    public string EnginePath = "C:\\Users\\";
}