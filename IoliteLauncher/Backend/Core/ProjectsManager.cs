using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using IoLiteLauncher.Backend;
using IoLiteLauncher.Utils;

namespace IoliteLauncher.Backend.Core;

public class ProjectsManager {
    private readonly SettingsManager _settingsManager;


    private string EnginePath => _settingsManager.SettingsData.EnginePath;

    public string ProjectDataStoragePath =>
        Path.Combine(_settingsManager.SettingsData.EnginePath, Statics.ProjectDataStructureName);

    public ProjectsManager() {
        var instance = Instance.Get;
        _settingsManager = instance.SettingsManager;
        _lockManager = instance.LockManager;
    }


    private string ProjectDefaultTemplatePath => Path.Combine(Statics.AppDataPath, Statics.DefaultProjectTemplateFolderName);

    public bool CreateProject(ProjectCreateOptions options) {
        var destPath = Path.Combine(_settingsManager.SettingsData.ProjectsPaths[0], options.Name);
        if (Directory.Exists(destPath)) {
            MessageBox.Show("Project path already exists!");
            return false;
        }

        if (!Directory.Exists(ProjectDefaultTemplatePath)) {
            MessageBox.Show("Default projects not found please reinstall using the installer.");
            return false;
        }
        try {
            Directory.CreateDirectory(destPath);

            Utils.MoveDirectoryContents(ProjectDefaultTemplatePath, destPath, true);

            ReplaceMetadataFileContents(options, destPath);

            OpenProject(destPath);
            return true;
        }
        catch (Exception e) {
            MessageBox.Show("Could not create project with E: " + e.Message);
            return false;
        }
    }

    private static void ReplaceMetadataFileContents(ProjectCreateOptions options, string destPath) {
        var metadataFile = Path.Combine(destPath, Statics.MetadataFileName);
        string contents = File.ReadAllText(metadataFile);
        contents = contents.Replace("%project_name%", options.Name);
        contents = contents.Replace("%organization_name%", options.OrgName);
        File.WriteAllText(metadataFile, contents);
    }

    public struct ProjectCreateOptions {
        public string Name;
        public bool OpenAfterCreation;
        public string OrgName;
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
            if (MetadataMgmt.IsMetaDataFile(file)) {
                var meta = MetadataMgmt.GetProjectMetaDataAtPath(file);
                if (meta == null) {
                    Debug.WriteLine("Project is null, skipping.");
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

    private readonly LockManager _lockManager;

    public void OpenProject(string projPath) {
        if (_lockManager.ContainsLock()) {
            var result =
                MessageBox.Show("Looks like another Project is currently open. Do you want to try force closing it?",
                    "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                bool successfullyClosed = CloseProject();
                if (successfullyClosed) OpenProject(projPath);
            }

            return;
        }

        bool successLock = _lockManager.CreateLock(new LockFile(projPath));
        if (!successLock) {
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

        bool success = StartCopy(projPath);
        if (!success) {
            Debug.WriteLine("Engine copy failed, aborting..");
            return;
        }

        Debug.WriteLine("Starting Engine..");
        StartEngine();
    }

    public Process? EngineProcess;
    public void StartEngine() {
        if (EngineProcess != null && EngineProcess.IsRunning()) {
            MessageBox.Show("Tired to start already running engine?!");
            return;
        }

        ProcessStartInfo info = new ProcessStartInfo() {
            WorkingDirectory = EnginePath,
            FileName = _settingsManager.ExecutablePath,
        };
        EngineProcess = new Process();
        EngineProcess.StartInfo = info;
        EngineProcess.EnableRaisingEvents = true;

        EngineProcess.Exited += OnEngineExit;
        EngineProcess.Disposed += OnEngineExit;

        EngineProcess.Start();

        if (!EngineProcess.IsRunning()) {
            MessageBox.Show("Could not start Engine!");
            CloseProject();
            return;
        }
    }

    private void OnEngineExit(object? sender, EventArgs e) {
        Debug.WriteLine("Engine closed, closing projects!");
        CloseProject();
    }

    private bool StartCopy(string projPath) {
        var moveDirectoryContents = Utils.MoveDirectoryContents(projPath, EnginePath);

        if (!moveDirectoryContents.success) {
            MessageBox.Show("Could not start copying directory contents.");
            return false;
        }

        bool writeResult = Serializer.ToFile(ProjectDataStoragePath, moveDirectoryContents.projectDataStorage);
        if (!writeResult) {
            MessageBox.Show("Could not write project data storage path.");
            return false;
        }

        return true;
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

    public bool CloseProject() {
        if (!File.Exists(ProjectDataStoragePath)) {
            Debug.WriteLine("Project data storage not found, aborting & deleting .lock");
            _lockManager.ForceRemoveLockFile();
            return false;
        }

        ProjectDataStorage? storage = Serializer.FromFile<ProjectDataStorage>(ProjectDataStoragePath);
        if (storage == null) {
            MessageBox.Show("Storage null aborting closing project.");
            return false;
        }


        try {
            foreach (string dir in storage.Value.AffectedDirs) {
                var srcPath = Path.Combine(EnginePath, dir);
                var destPath = Path.Combine(storage.Value.ProjectPath, dir);
                Directory.Move(srcPath, destPath);
            }

            foreach (string file in storage.Value.AffectedFiles) {
                var srcPath = Path.Combine(EnginePath, file);
                var destPath = Path.Combine(storage.Value.ProjectPath, file);
                File.Move(srcPath, destPath);
            }

            File.Delete(ProjectDataStoragePath);
            return _lockManager.ForceRemoveLockFile();
        }
        catch (Exception e) {
            MessageBox.Show("Could not close project with e " + e.Message);
            return false;
        }
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

                    string destPath = Path.Combine(_settingsManager.SettingsData.ProjectsPaths[0], "TemplateProjects");
                    if (!Directory.Exists(destPath)) Directory.CreateDirectory(destPath);
                    if (allowed) {
                        foreach (var file in Directory.GetFiles(_settingsManager.SettingsData.EnginePath)) {
                            if (Statics.AffectedFileNames.Contains(Path.GetFileName(file))) {
                                File.Move(file, Path.Combine(destPath, Path.GetFileName(file)));
                            }
                        }
                    }

                    Directory.Move(dir, Path.Combine(destPath, dirName));
                }
                catch (Exception e) {
                    MessageBox.Show("Could not project. " + e.Message);
                }
            }
        }
    }
}