using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Documents;
using IoLiteLauncher.Backend;
using IoLiteLauncher.Utils;

namespace IoliteLauncher.Backend.Core;

public class ProjectsManager {
    private Instance _instance;
    private SettingsManager _settingsManager;


    private string EnginePath => _settingsManager.SettingsData.EnginePath;

    public ProjectsManager() {
        _instance = Instance.Get;
        _settingsManager = _instance.SettingsManager;
        _lockManager = _instance.LockManager;
    }

    public void CreateProject() {
    }

    public void DeleteProject() {
    }

    public void RenameProject(string projectPath, string newName) {
    }

    public List<ProjectData> FetchProjects() {
        List<ProjectData> pr = new List<ProjectData>();
        foreach (var path in _settingsManager.SettingsData.ProjectsPaths) {
            pr.AddRange(GetProjectsAtPath(path));
        }

        return pr;
    }

    private List<ProjectData> GetProjectsAtPath(string path) {
        List<ProjectData> pr = new List<ProjectData>();
        var result = GetProjectDataAtPath(path);
        if (result.success) pr.Add(result.data);

        foreach (string directory in Directory.GetDirectories(path)) {
            var r = GetProjectDataAtPath(directory);
            if (r.success) pr.Add(r.data);
        }

        return pr;
    }

    private (ProjectData data, bool success) GetProjectDataAtPath(string path) {
        foreach (var file in Directory.GetFiles(path)) {
            if (Utils.IsMetaDataFile(file)) {
                var meta = MetadataMgmt.GetProjectMetaDataAtPath(file);
                if (meta == null) {
                    Debug.WriteLine("Project is null, scipping.");
                    continue;
                }

                return (new ProjectData() {
                    Path = path,
                    ProjectName = meta.application_name,
                }, true);
            }
        }

        return (default, false);
    }


    public struct ProjectData {
        public string Path { get; set; }
        public string ProjectName { get; set; }
    }

    private LockManager _lockManager;

    public void OpenProject(string projPath) {
        if (_lockManager.ContainsLock()) {
            var result =
                MessageBox.Show("Looks like another Project is currently open. Do you want to try force closing it?",
                    "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                CloseProject();
            }

            return;
        }

        bool success = _lockManager.CreateLock(new LockFile(projPath));
        if (!success) {
            MessageBox.Show("Could not create lock file");
            return;
        }

        if (Utils.DoesDirContainFiles(_settingsManager.SettingsData.EnginePath, Statics.AffectedFileNames)) {
            _lockManager.ForceRemoveLockFile();
            MessageBox.Show(
                "There are still project files in the Engine Path. Please remove / backup the following files: " +
                String.Join(", ", Statics.AffectedFileNames));
            return;
        }

        StartCopy(projPath);

        Debug.WriteLine("Starting Engine..");
        Process.Start(_settingsManager.ExecutablePath);
    }


    private void StartCopy(string projPath) {
        var dirs = Directory.GetDirectories(projPath);
        var files = Directory.GetFiles(projPath);
        foreach (string dir in dirs) {
            Utils.MoveDirectory(dir, EnginePath);
        }

        foreach (string file in files) {
            File.Move(file, Path.Combine(EnginePath, Path.GetFileName(file)));
        }
    }


    public struct ProjectDataStorage {
        public string ProjectPath;
        public List<string> AffectedFiles;
        public List<string> AffectedDirs;

        public ProjectDataStorage(List<string> affectedDirs, List<string> affectedFiles, string projectPath) {
            AffectedDirs = affectedDirs;
            AffectedFiles = affectedFiles;
            ProjectPath = projectPath;
        }
    }

    private void CloseProject() {
    }


    public void MoveTemplateProjects() {
        for (int i = 0; i < Statics.TemplatePaths.Count; i++) {
            Statics.TemplatePaths[i] = Statics.TemplatePaths[i].ToLower();
        }

        bool allowed = false;
        foreach (var dir in Directory.GetDirectories(_settingsManager.SettingsData.EnginePath)) {
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            var dirName = dirInfo.Name;

            if (Statics.TemplatePaths.Contains(dirName.ToLower())) {
                try {
                    if (!allowed) {
                        var result = MessageBox.Show(
                            "In order to continue we need to move the Template projects. Would you like to continue?",
                            "", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.No) {
                            Application.Current.Shutdown();
                            return;
                        }

                        allowed = true;
                    }

                    string destiPath = Path.Combine(_settingsManager.SettingsData.ProjectsPaths[0], "TemplateProjects");
                    if (!Directory.Exists(destiPath)) Directory.CreateDirectory(destiPath);
                    if (allowed) {
                        foreach (var file in Directory.GetFiles(_settingsManager.SettingsData.EnginePath)) {
                            if (Utils.IsMetaDataFile(file)) {
                                File.Move(file, Path.Combine(destiPath, Path.GetFileName(file)));
                            }
                        }
                    }

                    Directory.Move(dir, Path.Combine(destiPath, dirName));
                }
                catch (Exception e) {
                    MessageBox.Show("Could not project. " + e.Message);
                }
            }
        }
    }
}