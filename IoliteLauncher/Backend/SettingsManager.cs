using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using IoliteLauncher.Views;

namespace IoLiteLauncher.Backend;

public class SettingsManager {
    public SettingsData SettingsData = new SettingsData();

    public string ExecutablePath => Path.Combine(SettingsData.EnginePath + ExecutableName);

    public readonly string ExecutableName = "Iolite.exe";

    private string _settingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "IOLauncher", "settings.json");

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
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
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
                if (Path.GetFileName(file).ToLower().Equals(ExecutableName.ToLower())) {
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